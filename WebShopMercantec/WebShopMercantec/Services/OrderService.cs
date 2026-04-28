using WebShopMercantec.Exceptions;
using WebShopMercantec.Mapping;
using WebShopMercantec.Models;
using WebShopMercantec.Repositories;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Services;

// manages the full checkout and order lifecycle:
// creation -> (approval -> asset assignment) | (decline -> refund) | (cancellation -> refund)
public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICreditService _creditService;
    private readonly ILogger<OrderService> _logger;

    // snipe-it status id 3 means 'deployed' or 'checked out'
    private const int StatusDeployed = 3;

    public OrderService(
        IUnitOfWork unitOfWork,
        ICreditService creditService,
        ILogger<OrderService> logger)
    {
        _unitOfWork = unitOfWork;
        _creditService = creditService;
        _logger = logger;
    }

    // ─ Create 

    public async Task<OrderDto> CreateOrderAsync(int userId, OrderCreateDto dto)
    {
        _logger.LogInformation("User {UserId} creating order for {Type} #{Id}", userId, dto.RequestableType, dto.RequestableId);

        decimal price = 0m;
        string productName;

        // validate asset or accessory availability and get current price
        if (dto.RequestableType == "asset")
        {
            var product = await _unitOfWork.Products.GetByIdAsync((uint)dto.RequestableId);
            if (product == null) throw new NotFoundException("Product", dto.RequestableId);
            if (!await _unitOfWork.Products.IsAvailableForCheckoutAsync((uint)dto.RequestableId))
                throw new ProductNotAvailableException(dto.RequestableId, "Not available for checkout");
            price = product.PurchaseCost ?? 0m;
            productName = product.Name ?? $"Asset #{dto.RequestableId}";
        }
        else if (dto.RequestableType == "accessory")
        {
            var accessory = await _unitOfWork.Accessories.GetByIdAsync((uint)dto.RequestableId);
            if (accessory == null) throw new NotFoundException("Accessory", dto.RequestableId);
            if (!await _unitOfWork.Accessories.IsAvailableAsync((uint)dto.RequestableId, dto.Quantity))
                throw new ProductNotAvailableException(dto.RequestableId, "Accessory not available in requested quantity");
            price = (accessory.PurchaseCost ?? 0m) * dto.Quantity;
            productName = accessory.Name ?? $"Accessory #{dto.RequestableId}";
        }
        else
        {
            throw new BadRequestException($"Unknown requestable type: {dto.RequestableType}");
        }

        // check if user can afford the purchase
        if (!await _creditService.HasSufficientCreditsAsync((uint)userId, price))
            throw new InsufficientCreditsException(price, await _creditService.GetBalanceAsync((uint)userId));

        // deduct credits immediately on order creation (deferred save)
        if (price > 0)
            await _creditService.DeductCreditsAsync((uint)userId, price, $"Purchase: {productName}", null, false);

        // log the checkout request in the db
        var order = new CheckoutRequest
        {
            UserId = userId,
            RequestableId = dto.RequestableId,
            RequestableType = dto.RequestableType == "asset"
                ? "App\\Models\\Asset"
                : "App\\Models\\Accessory",
            Quantity = dto.Quantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Orders.AddAsync(order);
        await _unitOfWork.SaveChangesAsync(); // execution strategy retries implicit transactions

        _logger.LogInformation("Order #{OrderId} created for user {UserId}", order.Id, userId);
        var user = await _unitOfWork.Users.GetByIdAsync((uint)userId);
        return OrderMapping.MapToDto(order, user, productName);
    }

    // ─ Read 

    public async Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int userId)
    {
        var orders = await _unitOfWork.Orders.GetUserOrdersAsync(userId);
        return orders.Select(o => OrderMapping.MapToDto(o));
    }

    public async Task<(IEnumerable<OrderDto> Orders, int TotalCount)> GetMyOrdersPagedAsync(
        int userId, int page, int pageSize)
    {
        var (orders, total) = await _unitOfWork.Orders.GetUserOrdersPagedAsync(userId, page, pageSize);
        return (orders.Select(o => OrderMapping.MapToDto(o)), total);
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int orderId, int requestingUserId, bool isAdmin)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync((uint)orderId);
        if (order == null || order.DeletedAt != null) throw new NotFoundException("Order", orderId);
        if (!isAdmin && order.UserId != requestingUserId) throw new ForbiddenException("Access denied");
        return OrderMapping.MapToDto(order);
    }

    // ─ Cancel (user) 

    public async Task<OrderDto> CancelOrderAsync(int orderId, int userId)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync((uint)orderId);
        if (order == null || order.DeletedAt != null) throw new NotFoundException("Order", orderId);
        if (order.UserId != userId) throw new ForbiddenException("Cannot cancel someone else's order");
        if (order.FulfilledAt != null) throw new BadRequestException("Cannot cancel a fulfilled order");
        if (order.CanceledAt != null) throw new BadRequestException("Order is already canceled");

        // refund user credits on cancellation
        var price = await GetOrderPriceAsync(order);
        if (price > 0)
            await _creditService.AddCreditsAsync((uint)userId, price, $"Refund: Order #{orderId} canceled", orderId, saveChanges: false);

        order.CanceledAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync(); // atomic transaction

        _logger.LogInformation("Order #{OrderId} canceled by user {UserId}", orderId, userId);
        return OrderMapping.MapToDto(order);
    }

    // ─ Admin: Approve 

    public async Task<OrderDto> ApproveOrderAsync(int orderId)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync((uint)orderId);
        if (order == null || order.DeletedAt != null) throw new NotFoundException("Order", orderId);
        if (order.FulfilledAt != null) throw new BadRequestException("Order already fulfilled");
        if (order.CanceledAt != null) throw new BadRequestException("Cannot approve a canceled order");

        // if it's an asset, mark it as deployed to the user in Snipe-IT
        if (order.RequestableType.Contains("Asset"))
        {
            var asset = await _unitOfWork.Products.GetByIdAsync((uint)order.RequestableId);
            if (asset != null)
            {
                asset.AssignedTo = order.UserId;
                asset.AssignedType = "App\\Models\\User";
                asset.StatusId = StatusDeployed;
                asset.LastCheckout = DateTime.UtcNow;
                asset.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Products.Update(asset);
            }
        }

        order.FulfilledAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Order #{OrderId} approved", orderId);
        return OrderMapping.MapToDto(order);
    }

    // ─ Admin: Decline 

    public async Task<OrderDto> DeclineOrderAsync(int orderId, string? reason = null)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync((uint)orderId);
        if (order == null || order.DeletedAt != null) throw new NotFoundException("Order", orderId);
        if (order.FulfilledAt != null) throw new BadRequestException("Cannot decline a fulfilled order");
        if (order.CanceledAt != null) throw new BadRequestException("Order already canceled");

        // refund credits if order is declined by admin
        var price = await GetOrderPriceAsync(order);
        if (price > 0)
            await _creditService.AddCreditsAsync(
                (uint)order.UserId, price,
                $"Refund: Order #{orderId} declined. {reason}".TrimEnd('.', ' '),
                orderId, saveChanges: false);

        order.CanceledAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Orders.Update(order);
        await _unitOfWork.SaveChangesAsync(); // safe atomic trans commit

        _logger.LogInformation("Order #{OrderId} declined", orderId);
        return OrderMapping.MapToDto(order);
    }

    public async Task<(IEnumerable<OrderDto> Orders, int TotalCount)> GetAllOrdersPagedAsync(
        int page, int pageSize, string? status = null)
    {
        var (orders, total) = await _unitOfWork.Orders.GetAllOrdersPagedAsync(page, pageSize, status);
        return (orders.Select(o => OrderMapping.MapToDto(o)), total);
    }

    // ─ Helpers 

    // calculates total order price based on asset cost or accessory unit cost * quantity
    private async Task<decimal> GetOrderPriceAsync(CheckoutRequest order)
    {
        if (order.RequestableType.Contains("Asset"))
        {
            var asset = await _unitOfWork.Products.GetByIdAsync((uint)order.RequestableId);
            return asset?.PurchaseCost ?? 0m;
        }
        if (order.RequestableType.Contains("Accessory"))
        {
            var acc = await _unitOfWork.Accessories.GetByIdAsync((uint)order.RequestableId);
            return (acc?.PurchaseCost ?? 0m) * order.Quantity;
        }
        return 0m;
    }
}


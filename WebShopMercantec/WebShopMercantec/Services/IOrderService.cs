using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Services;

public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(int userId, OrderCreateDto dto);
    Task<IEnumerable<OrderDto>> GetMyOrdersAsync(int userId);
    Task<(IEnumerable<OrderDto> Orders, int TotalCount)> GetMyOrdersPagedAsync(int userId, int page, int pageSize);
    Task<OrderDto?> GetOrderByIdAsync(int orderId, int requestingUserId, bool isAdmin);
    Task<OrderDto> CancelOrderAsync(int orderId, int userId);
    // Admin
    Task<(IEnumerable<OrderDto> Orders, int TotalCount)> GetAllOrdersPagedAsync(int page, int pageSize, string? status = null);
    Task<OrderDto> ApproveOrderAsync(int orderId);
    Task<OrderDto> DeclineOrderAsync(int orderId, string? reason = null);
}


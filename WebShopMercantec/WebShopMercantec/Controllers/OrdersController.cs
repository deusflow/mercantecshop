using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShopMercantec.Extensions;
using WebShopMercantec.Services;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Controllers;

/// <summary>
/// POST /api/orders                  → создать заказ
/// GET  /api/orders/my               → мои заказы
/// GET  /api/orders/my/paged         → мои заказы с пагинацией
/// GET  /api/orders/{id}             → детали заказа
/// POST /api/orders/{id}/cancel      → отменить заказ (пользователь)
/// GET  /api/orders                  → [Admin] все заказы
/// POST /api/orders/{id}/approve     → [Admin] одобрить
/// POST /api/orders/{id}/decline     → [Admin] отклонить
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>Create a new checkout request</summary>
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] OrderCreateDto dto)
    {
        var userId = User.GetUserId();
        var order = await _orderService.CreateOrderAsync(userId, dto);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>Get my orders (all)</summary>
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetMy()
    {
        var userId = User.GetUserId();
        var orders = await _orderService.GetMyOrdersAsync(userId);
        return Ok(orders);
    }

    /// <summary>Get my orders paginated</summary>
    [HttpGet("my/paged")]
    public async Task<ActionResult> GetMyPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = User.GetUserId();
        var (orders, total) = await _orderService.GetMyOrdersPagedAsync(userId, page, pageSize);
        return Ok(new
        {
            items = orders,
            totalCount = total,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
    }

    /// <summary>Get order by ID</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var userId = User.GetUserId();
        var isAdmin = User.IsAdmin();
        var order = await _orderService.GetOrderByIdAsync(id, userId, isAdmin);
        return Ok(order);
    }

    /// <summary>Cancel my order</summary>
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<OrderDto>> Cancel(int id)
    {
        var userId = User.GetUserId();
        var order = await _orderService.CancelOrderAsync(id, userId);
        return Ok(order);
    }

    // ── Admin endpoints ───────────────────────────────────────────────────

    /// <summary>Get all orders paginated [Admin]</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        var (orders, total) = await _orderService.GetAllOrdersPagedAsync(page, pageSize, status);
        return Ok(new
        {
            items = orders,
            totalCount = total,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
    }

    /// <summary>Approve an order [Admin]</summary>
    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<OrderDto>> Approve(int id)
    {
        var order = await _orderService.ApproveOrderAsync(id);
        return Ok(order);
    }

    /// <summary>Decline an order and refund credits [Admin]</summary>
    [HttpPost("{id}/decline")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<OrderDto>> Decline(int id, [FromBody] string? reason = null)
    {
        var order = await _orderService.DeclineOrderAsync(id, reason);
        return Ok(order);
    }
}


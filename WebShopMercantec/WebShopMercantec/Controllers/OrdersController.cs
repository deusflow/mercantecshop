using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShopMercantec.Extensions;
using WebShopMercantec.Services;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Controllers;

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

    // create new order
    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] OrderCreateDto dto)
    {
        var userId = User.GetUserId();
        var order = await _orderService.CreateOrderAsync(userId, dto);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    // get all my orders
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetMy()
    {
        var userId = User.GetUserId();
        var orders = await _orderService.GetMyOrdersAsync(userId);
        return Ok(orders);
    }

    // get my orders with paging
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

    // get order by id (checks owner/admin)
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var userId = User.GetUserId();
        var isAdmin = User.IsAdmin();
        var order = await _orderService.GetOrderByIdAsync(id, userId, isAdmin);
        return Ok(order);
    }

    // cancel own order
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult<OrderDto>> Cancel(int id)
    {
        var userId = User.GetUserId();
        var order = await _orderService.CancelOrderAsync(id, userId);
        return Ok(order);
    }

    // admin: list all orders
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

    // admin: approve order
    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<OrderDto>> Approve(int id)
    {
        var order = await _orderService.ApproveOrderAsync(id);
        return Ok(order);
    }

    // admin: decline order and refund
    [HttpPost("{id}/decline")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<OrderDto>> Decline(int id, [FromBody] string? reason = null)
    {
        var order = await _orderService.DeclineOrderAsync(id, reason);
        return Ok(order);
    }
}

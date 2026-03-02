using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShopMercantec.Extensions;
using WebShopMercantec.Services;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Controllers;

/// <summary>
/// GET  /api/users/me              → профиль текущего пользователя
/// PUT  /api/users/me              → обновить профиль
/// GET  /api/users/{id}            → [Admin] профиль пользователя
/// GET  /api/users                 → [Admin] список пользователей
/// GET  /api/users/me/credits      → баланс кредитов
/// GET  /api/users/me/transactions → история транзакций
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICreditService _creditService;

    public UsersController(IUserService userService, ICreditService creditService)
    {
        _userService = userService;
        _creditService = creditService;
    }

    /// <summary>Get current user profile</summary>
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetMe()
    {
        var userId = User.GetUserId();
        var user = await _userService.GetCurrentUserAsync(userId);
        return Ok(user);
    }

    /// <summary>Update current user profile</summary>
    [HttpPut("me")]
    public async Task<ActionResult<UserDto>> UpdateMe([FromBody] UserDto dto)
    {
        var userId = User.GetUserId();
        var updated = await _userService.UpdateProfileAsync(userId, dto);
        return Ok(updated);
    }

    /// <summary>Get user by ID [Admin only]</summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        return Ok(user);
    }

    /// <summary>Get paginated users list [Admin only]</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
    {
        var (users, total) = await _userService.GetUsersPagedAsync(page, pageSize, search);
        return Ok(new
        {
            items = users,
            totalCount = total,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
    }

    /// <summary>Get current user's credit balance</summary>
    [HttpGet("me/credits")]
    public async Task<ActionResult> GetMyCredits()
    {
        var userId = User.GetUserId();
        var balance = await _creditService.GetBalanceAsync((uint)userId);
        return Ok(new { userId, balance });
    }

    /// <summary>Get current user's transaction history</summary>
    [HttpGet("me/transactions")]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetMyTransactions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = User.GetUserId();
        var txs = await _creditService.GetTransactionHistoryAsync((uint)userId, page, pageSize);
        return Ok(txs);
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShopMercantec.Extensions;
using WebShopMercantec.Services;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Controllers;

/// <summary>
/// POST /api/auth/login        → получить JWT
/// POST /api/auth/register     → регистрация
/// POST /api/auth/refresh      → обновить токен
/// POST /api/auth/logout       → отозвать refresh token
/// GET  /api/auth/me           → текущий пользователь
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>Login and receive JWT access + refresh token</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(result);
    }

    /// <summary>Register a new user account</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        if (dto.Password != dto.ConfirmPassword)
            return BadRequest(new { message = "Passwords do not match" });

        var result = await _authService.RegisterAsync(dto);
        return CreatedAtAction(nameof(Me), result);
    }

    /// <summary>Refresh access token using refresh token</summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] RefreshTokenDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
        return Ok(result);
    }

    /// <summary>Revoke refresh token (logout)</summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout([FromBody] RefreshTokenDto dto)
    {
        await _authService.RevokeTokenAsync(dto.RefreshToken);
        return NoContent();
    }

    /// <summary>Get the currently authenticated user</summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> Me()
    {
        var userId = User.GetUserId();
        var user = await _authService.GetCurrentUserAsync(userId);
        if (user == null) return Unauthorized();
        return Ok(user);
    }
}


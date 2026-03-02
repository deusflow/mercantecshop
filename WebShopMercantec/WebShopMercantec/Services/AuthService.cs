using WebShopMercantec.Configuration;
using WebShopMercantec.Exceptions;
using WebShopMercantec.Mapping;
using WebShopMercantec.Models;
using WebShopMercantec.Repositories;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Services;

/// <summary>
/// Handles login, register, refresh token and "who am I" lookups.
/// Snipe-IT passwords use Laravel bcrypt ($2y$ prefix) — we convert to $2a$ for BCrypt.Net.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        JwtSettings jwtSettings,
        ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings;
        _logger = logger;
    }

    // ─── Login ────────────────────────────────────────────────────────────

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        _logger.LogInformation("Login attempt for: {Username}", dto.Username);

        var user = await _unitOfWork.Users.GetByEmailOrUsernameAsync(dto.Username);

        if (user == null || user.DeletedAt != null)
            throw new UnauthorizedException("Invalid username or password");

        if (!user.Activated)
            throw new UnauthorizedException("Account is not activated");

        if (!VerifyPassword(dto.Password, user.Password))
            throw new UnauthorizedException("Invalid username or password");

        // Update last login
        var trackedUser = await _unitOfWork.Users.GetByIdAsync(user.Id);
        if (trackedUser != null)
        {
            trackedUser.LastLogin = DateTime.UtcNow;
            _unitOfWork.Users.Update(trackedUser);
        }

        var (accessToken, refreshToken, expiresAt) = await CreateTokenPairAsync(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User {UserId} logged in", user.Id);

        return BuildAuthResponse(user, accessToken, refreshToken, expiresAt);
    }

    // ─── Register ─────────────────────────────────────────────────────────

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        _logger.LogInformation("Register attempt for: {Username}", dto.Username);

        if (await _unitOfWork.Users.EmailExistsAsync(dto.Email))
            throw new BadRequestException($"Email '{dto.Email}' is already registered");

        if (await _unitOfWork.Users.UsernameExistsAsync(dto.Username))
            throw new BadRequestException($"Username '{dto.Username}' is already taken");

        // Hash password using BCrypt ($2a$ — compatible with Snipe-IT $2y$ on verify)
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: 12);

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = hashedPassword,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Phone = dto.Phone,
            Jobtitle = dto.JobTitle,
            LocationId = dto.LocationId,
            DepartmentId = dto.DepartmentId,
            Activated = true,
            ActivatedAt = DateTime.UtcNow,
            ShowInList = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Permissions = "{}"
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Init credit balance
        var credits = new WebShopUserCredits
        {
            UserId = user.Id,
            AvailableCredits = 0m,
            TotalSpent = 0m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Context.WebShopUserCredits.AddAsync(credits);

        var (accessToken, refreshToken, expiresAt) = await CreateTokenPairAsync(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("New user registered: {UserId}", user.Id);

        return BuildAuthResponse(user, accessToken, refreshToken, expiresAt);
    }

    // ─── Refresh ──────────────────────────────────────────────────────────

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var stored = await _unitOfWork.Context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken);

        if (stored == null || !stored.IsActive)
            throw new UnauthorizedException("Invalid or expired refresh token");

        var user = await _unitOfWork.Users.GetByIdAsync(stored.UserId);
        if (user == null || user.DeletedAt != null)
            throw new UnauthorizedException("User not found");

        // Rotate: revoke old, issue new
        stored.RevokedAt = DateTime.UtcNow;

        var (accessToken, newRefreshToken, expiresAt) = await CreateTokenPairAsync(user);
        stored.ReplacedByToken = newRefreshToken;

        _unitOfWork.Context.RefreshTokens.Update(stored);
        await _unitOfWork.SaveChangesAsync();

        return BuildAuthResponse(user, accessToken, newRefreshToken, expiresAt);
    }

    // ─── Revoke ───────────────────────────────────────────────────────────

    public async Task RevokeTokenAsync(string refreshToken)
    {
        var stored = await _unitOfWork.Context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken);

        if (stored == null || !stored.IsActive)
            throw new UnauthorizedException("Token not found or already revoked");

        stored.RevokedAt = DateTime.UtcNow;
        _unitOfWork.Context.RefreshTokens.Update(stored);
        await _unitOfWork.SaveChangesAsync();
    }

    // ─── Current user ────────────────────────────────────────────────────

    public async Task<UserDto?> GetCurrentUserAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync((uint)userId);
        if (user == null || user.DeletedAt != null) return null;

        var credits = await _unitOfWork.Context.WebShopUserCredits
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == user.Id);

        return UserMapping.MapToDto(user, credits);
    }

    // ─── Helpers ─────────────────────────────────────────────────────────

    private async Task<(string accessToken, string refreshToken, DateTime expiresAt)>
        CreateTokenPairAsync(User user)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var rawRefresh = _tokenService.GenerateRefreshToken();

        var storedToken = new RefreshToken
        {
            UserId = user.Id,
            Token = rawRefresh,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryInDays),
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Context.RefreshTokens.AddAsync(storedToken);
        // SaveChanges is called by the caller

        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryInMinutes);
        return (accessToken, rawRefresh, expiresAt);
    }

    private static AuthResponseDto BuildAuthResponse(
        User user, string accessToken, string refreshToken, DateTime expiresAt)
    {
        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            User = UserMapping.MapToDto(user)
        };
    }

    /// <summary>
    /// Verify BCrypt password — handles Laravel's $2y$ prefix
    /// </summary>
    private static bool VerifyPassword(string plainPassword, string storedHash)
    {
        try
        {
            // Laravel uses $2y$, BCrypt.Net expects $2a$ — they are equivalent
            var normalized = storedHash.StartsWith("$2y$")
                ? "$2a$" + storedHash[4..]
                : storedHash;

            return BCrypt.Net.BCrypt.Verify(plainPassword, normalized);
        }
        catch
        {
            return false;
        }
    }
}


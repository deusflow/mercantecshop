using WebShopMercantec.Models;

namespace WebShopMercantec.Services;

/// <summary>
/// Generates and validates JWT access tokens + refresh tokens
/// </summary>
public interface ITokenService
{
    /// <summary>Generate a JWT access token for a user</summary>
    string GenerateAccessToken(User user);

    /// <summary>Generate a secure refresh token string</summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Validate a JWT and return the user ID from claims.
    /// Returns null if token is invalid or expired.
    /// </summary>
    int? GetUserIdFromToken(string token);

    /// <summary>
    /// Get role from token claims
    /// </summary>
    string? GetRoleFromToken(string token);
}


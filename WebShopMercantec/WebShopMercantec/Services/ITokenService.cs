using WebShopMercantec.Models;

namespace WebShopMercantec.Services;

// Generates and validates JWT access tokens + refresh tokens

public interface ITokenService
{
    //Generate a JWT access token for a user</summary>
    string GenerateAccessToken(User user);

    //Generate a secure refresh token string</summary>
    string GenerateRefreshToken();

    // Validate a JWT and return the user ID from claims.
    // Returns null if token is invalid or expired.
    
    int? GetUserIdFromToken(string token);

    // Get role from token claims
    
    string? GetRoleFromToken(string token);
}


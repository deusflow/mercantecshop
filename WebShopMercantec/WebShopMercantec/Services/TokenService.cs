using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebShopMercantec.Configuration;
using WebShopMercantec.Models;

namespace WebShopMercantec.Services;

/// <summary>
/// JWT token generation and validation
/// </summary>
public class TokenService : ITokenService
{
    private readonly JwtSettings _jwt;
    private readonly ILogger<TokenService> _logger;

    public TokenService(JwtSettings jwt, ILogger<TokenService> logger)
    {
        _jwt = jwt;
        _logger = logger;
    }

    public string GenerateAccessToken(User user)
    {
        var role = ResolveRole(user);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.Username ?? user.Email ?? user.Id.ToString()),
            new Claim(ClaimTypes.Role, role),
            new Claim("userId", user.Id.ToString()),
            new Claim("firstName", user.FirstName ?? string.Empty),
            new Claim("lastName", user.LastName ?? string.Empty)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_jwt.ExpiryInMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    public int? GetUserIdFromToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));

            handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwt.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = false // Allow reading expired tokens for refresh flow
            }, out var validatedToken);

            var jwt = (JwtSecurityToken)validatedToken;
            var userIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == "userId")?.Value
                           ?? jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            return userIdClaim != null && int.TryParse(userIdClaim, out var id) ? id : null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to validate token: {Message}", ex.Message);
            return null;
        }
    }

    public string? GetRoleFromToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }
        catch
        {
            return null;
        }
    }

    // ─── Helpers ────────────────────────────────────────────────────────────

    /// <summary>
    /// Determine role from Snipe-IT permissions JSON or superadmin flag.
    /// Snipe-IT stores permissions as JSON: {"superadmin":"1","admin":"1",...}
    /// </summary>
    private static string ResolveRole(User user)
    {
        if (user.Permissions == null) return "User";
        if (user.Permissions.Contains("\"superadmin\":\"1\"") ||
            user.Permissions.Contains("\"superadmin\": \"1\"")) return "Admin";
        if (user.Permissions.Contains("\"admin\":\"1\"") ||
            user.Permissions.Contains("\"admin\": \"1\"")) return "Admin";
        return "User";
    }
}


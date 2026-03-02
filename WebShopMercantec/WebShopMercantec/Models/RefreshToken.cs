namespace WebShopMercantec.Models;

/// <summary>
/// WebShop-specific: refresh tokens для JWT.
/// </summary>
public class RefreshToken
{
    public int Id { get; set; }
    public uint UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }
    public bool IsRevoked => RevokedAt != null;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
}


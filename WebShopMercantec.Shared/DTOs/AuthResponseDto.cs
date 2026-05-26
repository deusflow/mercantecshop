namespace WebShopMercantec.Shared.DTOs;

/// <summary>
/// DTO returned after successful login / token refresh
/// </summary>
public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = null!;
}

/// <summary>
/// DTO for refresh token request
/// </summary>
public class RefreshTokenDto
{
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// DTO for adding/deducting credits (admin action)
/// </summary>
public class CreditAdjustmentDto
{
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// DTO for creating a new order (checkout request)
/// </summary>
public class OrderCreateDto
{
    public int RequestableId { get; set; }
    public string RequestableType { get; set; } = "asset"; // "asset" | "accessory"
    public int Quantity { get; set; } = 1;
    public string? Notes { get; set; }
}


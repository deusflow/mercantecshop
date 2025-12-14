namespace WebShopMercantec.Shared.DTOs;

/// <summary>
/// DTO for Login request
/// Used for authenticating users and providing credentials
/// Contains username/email and password for login validation
/// </summary>
public class LoginDto
{
    public string Username { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
    
    public bool RememberMe { get; set; }
}
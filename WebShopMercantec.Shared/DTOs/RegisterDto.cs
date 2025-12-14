namespace WebShopMercantec.Shared.DTOs;

/// <summary>
/// DTO for User Registration
/// Used when creating a new user account
/// Contains all required information for new user signup
/// </summary>
public class RegisterDto
{
    public string Username { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
    
    public string ConfirmPassword { get; set; } = string.Empty;
    
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
    
    public string? Phone { get; set; }
    
    public string? JobTitle { get; set; }
    
    public int? LocationId { get; set; }
    
    public int? DepartmentId { get; set; }
}
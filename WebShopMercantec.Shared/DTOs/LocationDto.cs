namespace WebShopMercantec.Shared.DTOs;

/// <summary>
/// DTO for Location (physical location/office/warehouse)
/// Used for managing and displaying location information for assets, users, and inventory
/// Helps organize products and users by their physical location
/// </summary>
public class LocationDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? City { get; set; }
    
    public string? State { get; set; }
    
    public string? Country { get; set; }
    
    public string? Address { get; set; }
    
    public string? Address2 { get; set; }
    
    public string? Zip { get; set; }
    
    public string? Phone { get; set; }
    
    public string? Fax { get; set; }
    
    public int? ParentId { get; set; }
    
    public string? ParentName { get; set; }
    
    public string? Currency { get; set; }
    
    public int? ManagerId { get; set; }
    
    public string? ManagerName { get; set; }
    
    public string? Image { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}
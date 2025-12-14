namespace WebShopMercantec.Shared.DTOs;

/// <summary>
/// DTO for Supplier (vendors/suppliers of products and equipment)
/// Used for managing supplier information and tracking where products are purchased from
/// Helps maintain supplier contacts and purchasing relationships
/// </summary>
public class SupplierDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? Address { get; set; }
    
    public string? Address2 { get; set; }
    
    public string? City { get; set; }
    
    public string? State { get; set; }
    
    public string? Country { get; set; }
    
    public string? Zip { get; set; }
    
    public string? Phone { get; set; }
    
    public string? Fax { get; set; }
    
    public string? Email { get; set; }
    
    public string? Contact { get; set; }
    
    public string? Url { get; set; }
    
    public string? Notes { get; set; }
    
    public string? Image { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}
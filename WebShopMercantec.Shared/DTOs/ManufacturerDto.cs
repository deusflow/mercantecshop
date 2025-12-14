namespace WebShopMercantec.Shared.DTOs;

/// <summary>
/// DTO for Manufacturer (product manufacturers like Dell, HP, Lenovo, Apple, etc.)
/// Used for managing manufacturer information and linking products to their makers
/// Contains manufacturer contact details, support URLs, and warranty information
/// Helps users find manufacturer support and warranty lookup for products
/// </summary>
public class ManufacturerDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? Url { get; set; }
    
    public string? SupportUrl { get; set; }
    
    public string? WarrantyLookupUrl { get; set; }
    
    public string? SupportPhone { get; set; }
    
    public string? SupportEmail { get; set; }
    
    public string? Image { get; set; }
    
    public int ProductsCount { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}
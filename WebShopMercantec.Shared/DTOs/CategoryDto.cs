namespace WebShopMercantec.Shared.DTOs;

/// <summary>
/// DTO for Category (product categories like "Laptops", "Monitors", "Accessories")
/// Used for organizing and filtering products in the shop
/// Helps users navigate and find products by category
/// </summary>
public class CategoryDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? CategoryType { get; set; } // Match model field name: "asset", "accessory", "consumable", "component"
    
    public int ItemsCount { get; set; } // How many products in this category
    
    public string? Image { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}
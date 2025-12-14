namespace WebShopMercantec.Shared.DTOs;

/// <summary>
/// DTO for Model (product model specifications like "Dell Latitude 5590", "HP ProBook 450")
/// Used for managing product model information and specifications
/// Connects manufacturers with specific product models
/// </summary>
public class ModelDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? ModelNumber { get; set; }
    
    public int? ManufacturerId { get; set; }
    
    public string? ManufacturerName { get; set; }
    
    public int? CategoryId { get; set; }
    
    public string? CategoryName { get; set; }
    
    public int? MinAmt { get; set; } 
    
    public int? DepreciationId { get; set; }
    
    public int? Eol { get; set; } // Match model field name (End of Life)
    
    public string? Image { get; set; }
    
    public string? Notes { get; set; }
    
    public bool Requestable { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}
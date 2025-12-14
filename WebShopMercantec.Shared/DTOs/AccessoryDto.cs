namespace WebShopMercantec.Shared.DTOs;

/// <summary>
/// DTO for Accessory (computer peripherals, cables, adapters, etc.)
/// Used for displaying available accessories in the shop and managing accessory inventory
/// </summary>
public class AccessoryDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public int? CategoryId { get; set; }
    
    public string? CategoryName { get; set; }
    
    public int Qty { get; set; } 
    
    public bool Requestable { get; set; }
    
    public int? LocationId { get; set; }
    
    public string? LocationName { get; set; }
    
    public DateTime? PurchaseDate { get; set; }
    
    public decimal? PurchaseCost { get; set; }
    
    public string? OrderNumber { get; set; }
    
    public int? CompanyId { get; set; }
    
    public int? MinAmt { get; set; } 
    
    public int? ManufacturerId { get; set; }
    
    public string? ManufacturerName { get; set; }
    
    public string? ModelNumber { get; set; }
    
    public string? Image { get; set; }
    
    public int? SupplierId { get; set; }
    
    public string? SupplierName { get; set; }
    
    public string? Notes { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}
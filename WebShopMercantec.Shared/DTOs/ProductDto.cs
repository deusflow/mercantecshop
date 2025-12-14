namespace WebShopMercantec.Shared.DTOs;

/// <summary>
/// DTO for Product/Asset (main products like laptops, computers, equipment)
/// Used for displaying available products in the shop catalog
/// Contains all information needed to show and purchase products
/// </summary>
public class ProductDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string AssetTag { get; set; } = string.Empty;
    
    public string? Image { get; set; } 
    
    public int? ModelId { get; set; }
    
    public string? ModelName { get; set; } // For display
    
    public string? Serial { get; set; }
    
    public int? StatusId { get; set; }
    
    // Category (Laptops, Monitors, etc.)
    public string CategoryName { get; set; } = string.Empty;
    
    // Status (Ready to Deploy, In Repair, etc.)
    public string StatusLabel { get; set; } = string.Empty;

    // Description or notes
    public string? Notes { get; set; }

    // Price (purchase cost in credits)
    public decimal? PurchaseCost { get; set; }
    
    public decimal Price { get; set; } // Computed price for shop display
    
    public string? OrderNumber { get; set; }
    
    // Manufacturer information (through Model relationship)
    public int? ManufacturerId { get; set; }
    
    public string? ManufacturerName { get; set; }
    
    // Model information
    public string? ModelNumber { get; set; }
    
    // Location information
    public int? LocationId { get; set; }
    
    public string? LocationName { get; set; }
    
    public int? RtdLocationId { get; set; } // Ready to Deploy Location
    
    // Supplier
    public int? SupplierId { get; set; }
    
    public string? SupplierName { get; set; }
    
    // Company
    public int? CompanyId { get; set; }
    
    public string? CompanyName { get; set; }
    
    // Assignment
    public int? AssignedTo { get; set; }
    
    public string? AssignedType { get; set; }
    
    public string? AssignedToName { get; set; }
    
    // Availability
    public bool IsAvailable { get; set; } // Computed
    
    public bool Requestable { get; set; }
    
    public bool? Archived { get; set; }
    
    // Warranty
    public int? WarrantyMonths { get; set; }
    
    // Dates
    public DateTime? PurchaseDate { get; set; }
    
    public DateTime? AssetEolDate { get; set; } // End of Life
    
    public DateTime? LastCheckout { get; set; }
    
    public DateTime? LastCheckin { get; set; }
    
    public DateTime? ExpectedCheckin { get; set; }
    
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}
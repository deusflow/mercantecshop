namespace WebShopMercantec.Shared.DTOs;

/// <summary>
/// DTO for Status Label (product status like "Ready to Deploy", "In Repair", "Archived")
/// Used for managing and displaying product availability status
/// Helps users understand if a product is available for checkout
/// Contains color coding for visual status indication and deployment flags
/// Typical use cases:
/// - Getting list of statuses for filtering and displaying products
/// - Managing statuses in admin panel
/// - Displaying color labels and status descriptions
/// </summary>
public class StatusLabelDto
{
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? Color { get; set; }
    
    public bool Deployable { get; set; } // Can be assigned to users
    
    public bool Pending { get; set; } // Waiting for something
    
    public bool Archived { get; set; } // Archived/inactive
    
    public string? Notes { get; set; }
    
    public bool? ShowInNav { get; set; }
    
    public bool? DefaultLabel { get; set; }
    
    public int AssetsCount { get; set; } // Number of assets with this status
    
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}

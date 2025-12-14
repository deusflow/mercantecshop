namespace WebShopMercantec.Shared.DTOs;

/// <summary>
/// DTO for Checkout Acceptance (user acceptance of product checkout)
/// Used when a user accepts/confirms receipt of a product
/// Tracks acceptance signatures, EULA agreements, and acceptance timestamps
/// </summary>
public class CheckoutAcceptanceDto
{
    public int Id { get; set; }
    
    public string CheckoutableType { get; set; } = string.Empty; // e.g., "Asset", "Accessory"
    
    public int CheckoutableId { get; set; } 
    
    public int? AssignedToId { get; set; } 
    
    public string AssignedToUserName { get; set; } = string.Empty; // For display
    
    public DateTime? AcceptedAt { get; set; }
    
    public DateTime? DeclinedAt { get; set; }
    
    public string? SignatureFilename { get; set; } 
    
    public string? Note { get; set; }
    
    public string? StoredEula { get; set; } 
    
    public string? StoredEulaFile { get; set; } 
    
    public string Status { get; set; } = "Pending";
    
    public DateTime? CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}

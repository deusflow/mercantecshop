namespace WebShopMercantec.Shared.DTOs;

/// <summary>
/// DTO for Order/Checkout Request (user's purchase request)
/// Used for managing product checkout requests and tracking order status
/// Flow: User clicks "Purchase with Credits" → Create CheckoutRequest with status "Pending"
/// → Admin approves → status "Fulfilled" OR Admin cancels → status "Canceled"
/// </summary>
public class OrderDto
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public string UserName { get; set; } = string.Empty; // For display
    
    public int RequestableId { get; set; }
    
    public string RequestableType { get; set; } = string.Empty;
    
    public string ProductName { get; set; } = string.Empty; // For display
    
    public int Quantity { get; set; }
    
    // Price fields (not in CheckoutRequest model, calculated separately)
    public decimal Price { get; set; }
    
    public decimal TotalPrice { get; set; }
    
    // Status computed from dates
    public string Status { get; set; } = "Pending"; // Pending, Fulfilled, Canceled, Declined
    
    // Timestamps from model
    public DateTime? CreatedAt { get; set; } // Request creation time
    
    public DateTime? UpdatedAt { get; set; }
    
    public DateTime? FulfilledAt { get; set; }
    
    public DateTime? CanceledAt { get; set; }
    
    public string? Notes { get; set; } // Additional notes (not in model, can be added)
    
    public string? DeclineReason { get; set; } // Decline reason (not in model, can be added)
}


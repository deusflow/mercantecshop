namespace WebShopMercantec.Shared.DTOs;

/// <summary>
/// DTO for Transaction (credit transaction history)
/// Used for tracking user credit balance changes and transaction history
/// Records all credit additions, purchases, and refunds
/// </summary>
public class TransactionDto
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public string UserName { get; set; } = string.Empty;
    
    public string Type { get; set; } = string.Empty; // Purchase, AdminCredit, Refund, Adjustment
    
    public string ItemName { get; set; } = string.Empty;
    
    public decimal Amount { get; set; } // Positive for credits added, negative for purchases
    
    public decimal BalanceBefore { get; set; }
    
    public decimal BalanceAfter { get; set; }
    
    public string Status { get; set; } = "Completed";
    
    public DateTime CreatedAt { get; set; }
    
    public string? Description { get; set; }
    
    public int? RelatedOrderId { get; set; }
}

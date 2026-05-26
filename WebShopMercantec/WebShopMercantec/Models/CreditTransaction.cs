namespace WebShopMercantec.Models;

public class CreditTransaction
{
    public int Id { get; set; }
    public uint UserId { get; set; }
    public decimal Amount { get; set; }         // Positive = credit, negative = debit
    public string Type { get; set; } = string.Empty;  // "credit" | "debit" | "refund"
    public string? Reason { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public int? RelatedOrderId { get; set; }
    public DateTime CreatedAt { get; set; }
}

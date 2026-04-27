namespace WebShopMercantec.Models;

public class WebShopUserCredits
{
    public int Id { get; set; }
    public uint UserId { get; set; }
    public decimal AvailableCredits { get; set; }
    public decimal TotalSpent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}


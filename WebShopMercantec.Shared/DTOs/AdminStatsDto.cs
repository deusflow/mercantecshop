namespace WebShopMercantec.Shared.DTOs;

public class AdminStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public decimal TotalCredits { get; set; }
    public int TotalTransactions { get; set; }
}

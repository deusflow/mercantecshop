namespace WebShopMercantec.Shared.DTOs;

public class CreateDeviceDto
{
    public string Name { get; set; } = string.Empty;
    public string AssetTag { get; set; } = string.Empty;
    public int? ModelId { get; set; }
    public int StatusId { get; set; }
    public decimal? PurchaseCost { get; set; }
    public bool Requestable { get; set; } = true;
    public string? Notes { get; set; }
}

namespace WebShopMercantec.Models;

public class EnrichedAsset
{
    public Asset Asset { get; set; } = null!;
    public Model? Model { get; set; }
    public Category? Category { get; set; }
    public Manufacturer? Manufacturer { get; set; }
    public StatusLabel? StatusLabel { get; set; }
}


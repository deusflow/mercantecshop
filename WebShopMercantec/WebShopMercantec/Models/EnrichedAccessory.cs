namespace WebShopMercantec.Models;

public class EnrichedAccessory
{
    
    public Accessory Accessory { get; set; } = null!;
    
    
    public Category? Category { get; set; }
    
    
    public Manufacturer? Manufacturer { get; set; }
    
    
    public Supplier? Supplier { get; set; }
    
    
    public Location? Location { get; set; }
}


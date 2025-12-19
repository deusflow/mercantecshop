namespace WebShopMercantec.Models;

/// <summary>
/// Расширенная модель Asset с подгруженными связанными данными
/// Используется для маппинга в ProductDto с полной информацией
/// </summary>
public class EnrichedAsset
{
    public Asset Asset { get; set; } = null!;
    public Model? Model { get; set; }
    public Category? Category { get; set; }
    public Manufacturer? Manufacturer { get; set; }
    public StatusLabel? StatusLabel { get; set; }
}


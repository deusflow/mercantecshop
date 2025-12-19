namespace WebShopMercantec.Models;

/// <summary>
/// Расширенная модель Accessory с подгруженными связанными данными
/// Используется для маппинга в AccessoryDto с полной информацией
/// Решает проблему N+1 запросов - все данные загружаются одним запросом через JOIN
/// </summary>
public class EnrichedAccessory
{
    /// <summary>
    /// Основная сущность - аксессуар
    /// </summary>
    public Accessory Accessory { get; set; } = null!;
    
    /// <summary>
    /// Категория аксессуара (например, "Мыши", "Клавиатуры", "Кабели")
    /// </summary>
    public Category? Category { get; set; }
    
    /// <summary>
    /// Производитель (например, "Logitech", "Microsoft")
    /// </summary>
    public Manufacturer? Manufacturer { get; set; }
    
    /// <summary>
    /// Поставщик, у которого купили
    /// </summary>
    public Supplier? Supplier { get; set; }
    
    /// <summary>
    /// Местоположение (где хранится)
    /// </summary>
    public Location? Location { get; set; }
}


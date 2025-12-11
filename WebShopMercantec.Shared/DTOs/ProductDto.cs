namespace WebShopMercantec.Shared.DTOs;

public class ProductDto
{
    
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string AssetTag { get; set; } = string.Empty;
    
    
    public string? Image { get; set; } 
    
    public string? ModelName { get; set; } 
    
    public string? Serial { get; set; }
    
    public int StatusId { get; set; }
    
    // Категория (Ноутбуки, Мониторы...)
    public string CategoryName { get; set; } = string.Empty;
    
    // Статус (Готов к выдаче, В ремонте)
    public string StatusLabel { get; set; } = string.Empty;

    // Описание или заметки
    public string Notes { get; set; } = string.Empty;

    // Цена (В Snipe-IT цена часто в purchase_cost, но пока сделаем заглушку)
    public decimal Price { get; set; }
}
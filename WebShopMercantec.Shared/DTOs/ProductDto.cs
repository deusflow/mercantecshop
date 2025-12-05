namespace WebShopMercantec.Shared.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    
    // Название товара (например, "MacBook Pro 16")
    public string Name { get; set; } = string.Empty;
    
    // Инвентарный номер (чтобы кладовщик понимал, о чем речь)
    public string AssetTag { get; set; } = string.Empty;
    
    // Ссылка на картинку (Snipe-IT хранит их отдельно, но мы склеим путь)
    public string ImageUrl { get; set; } = string.Empty;
    
    // Категория (Ноутбуки, Мониторы...)
    public string CategoryName { get; set; } = string.Empty;
    
    // Статус (Готов к выдаче, В ремонте)
    public string StatusLabel { get; set; } = string.Empty;

    // Описание или заметки
    public string Notes { get; set; } = string.Empty;

    // Цена (В Snipe-IT цена часто в purchase_cost, но пока сделаем заглушку)
    public decimal Price { get; set; }
}
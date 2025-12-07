namespace WebShopMercantec.Shared.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // Например "asset", "accessory"
    public int ItemsCount { get; set; } // Сколько товаров в этой категории
}
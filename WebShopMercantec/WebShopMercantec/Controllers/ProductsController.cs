using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models; // Твои модели базы данных (Asset, SnipeItContext)
using WebShopMercantec.Shared.DTOs; // Твои DTO (ProductDto)

namespace WebShopMercantec.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly SnipeItContext _context;

    public ProductsController(SnipeItContext context)
    {
        _context = context;
    }

    // 1. GET: api/products (Получить список)
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts()
    {
        // Берем данные из базы
        var assets = await _context.Assets
            .Take(20)
            .AsNoTracking()
            .ToListAsync();

        if (assets == null || assets.Count == 0)
        {
            return NotFound("Товары не найдены");
        }

        // Превращаем каждый Asset в ProductDto
        var productDtos = assets.Select(asset => MapToDto(asset)).ToList();

        return Ok(productDtos);
    }

    // 2. GET: api/products/5 (Получить ОДИН товар)
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        // Ищем товар по ID
        var asset = await _context.Assets
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (asset == null)
        {
            return NotFound($"Товар с ID {id} не найден");
        }

        // Превращаем в DTO
        var productDto = MapToDto(asset);

        return Ok(productDto);
    }

    // Вспомогательный метод для маппинга (Asset -> ProductDto)
    private static ProductDto MapToDto(Asset asset)
    {
        return new ProductDto
        {
            // Явное приведение типов (int), чтобы компилятор не ругался на uint/long
            Id = (int)asset.Id,
            
            // Проверка на null (?? "...")
            Name = asset.Name ?? "Без названия",
            AssetTag = asset.AssetTag ?? "Нет инв. номера",
            
            // Если картинки нет, можно вернуть пустую строку или URL заглушки
            ImageUrl = asset.Image ?? "", 
            
            // Пока ставим заглушки, так как этих полей нет в таблице assets напрямую (нужен Join)
            StatusLabel = "Available", 
            CategoryName = "General",
            
            // Приведение decimal? к decimal
            Price = asset.PurchaseCost ?? 0, 
            
            // Записываем серийник в Notes, так как Description в DTO нет (или ты его не добавил)
            Notes = $"Серийный номер: {asset.Serial}" 
        };
    }
}
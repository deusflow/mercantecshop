using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models; 
using WebShopMercantec.Shared.DTOs; 

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

    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts()
    {
        // 1. Достаем "сырые" данные из базы
        var assets = await _context.Assets
            .Take(20)
            .AsNoTracking() 
            .ToListAsync();

        if (assets == null || assets.Count == 0)
        {
            return NotFound("Товары не найдены");
        }

        // 2. Маппинг (Превращение Asset -> ProductDto)
        var productDtos = assets.Select(asset => new ProductDto
        {
            // Если asset.Id вдруг long/uint, приводим к int принудительно
            Id = (int)asset.Id, 
            
            Name = asset.Name ?? "Без названия",
            AssetTag = asset.AssetTag ?? "",
            ImageUrl = asset.Image ?? "", 
            StatusLabel = "Available", 
            
            // PurchaseCost это decimal?, если null, ставим 0
            Price = asset.PurchaseCost ?? 0, 
            
            // В ProductDto нет поля Description, но есть Notes. Пишем туда.
            Notes = $"Серийный номер: {asset.Serial}", 
            
            CategoryName = "General" 
        }).ToList();

        return Ok(productDtos);
    }
}
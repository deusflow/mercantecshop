using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;

namespace WebShopMercantec.Controllers;

/// <summary>
/// API для работы с товарами (assets) из Snipe-IT.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly SnipeItContext _context;

    public ProductsController(SnipeItContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Возвращает список товаров (первые 20 записей из Snipe-IT Assets).
    /// </summary>
    /// <remarks>
    /// Используется для витрины вебшопа. В будущем можно добавить фильтры, пагинацию и маппинг в ProductDto.
    /// </remarks>
    /// <response code="200">Список товаров успешно получен.</response>
    /// <response code="404">Товары не найдены в базе.</response>
    [HttpGet]
    public async Task<ActionResult<List<Asset>>> GetProducts()
    {
        // Идем в базу -> Таблица Assets -> Берем первые 20 штук -> Превращаем в список
        var products = await _context.Assets.Take(20).ToListAsync();

        if (products.Count == 0)
        {
            return NotFound("Products not found"); // Возвращаем статус 404 Not Found, если нет данных
        }

        return Ok(products); // 200ok
    }
    
    /// <summary>
    /// Возвращает товар по ID.
    /// </summary>
    /// <param name="id">ID товара (Asset ID из Snipe-IT).</param>
    /// <response code="200">Товар найден.</response>
    /// <response code="404">Товар с указанным ID не найден.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<Asset>> GetProduct(int id)
    {
        var product = await _context.Assets.FindAsync(id);

        if (product == null)
        {
            return NotFound($"Product with ID {id} not found");
        }

        return Ok(product);
    }
}
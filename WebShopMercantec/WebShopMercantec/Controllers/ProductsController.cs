using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;

namespace WebShopMercantec.Controllers;

[ApiController] // API controller
[Route("api/[controller]")] // adress /api/products
public class ProductsController : ControllerBase // neste class
{
    private readonly SnipeItContext _context;

   
    public ProductsController(SnipeItContext context)
    {
        _context = context;
    }

    
    // Сработает при запросе GET http://localhost:xxxx/api/products
    [HttpGet]
    public async Task<ActionResult<List<Asset>>> GetProducts()
    {
        // Идем в базу -> Таблица Assets -> Берем первые 20 штук -> Превращаем в список
        var products = await _context.Assets.Take(20).ToListAsync();

        if (products == null)
        {
            return NotFound("Products not found"); // Возвращаем статус 404 Not Found, если нет данных
        }

        return Ok(products); // 200ok
    }
}
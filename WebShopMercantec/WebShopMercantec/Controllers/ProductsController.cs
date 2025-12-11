using Microsoft.AspNetCore.Mvc;
using WebShopMercantec.Services; // Подключаем наши сервисы
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        // Раньше тут был SnipeItContext. Теперь тут Сервис.
        private readonly IProductService _productService;

        // Конструктор: "Дайте мне сервис продуктов, пожалуйста"
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            // Официант просто передает заказ Повару
            var products = await _productService.GetAvailableProductsAsync();
            
            // И отдает результат Клиенту
            return Ok(products);
        }

        // Добавим метод получения одного товара (раз уж написали в сервисе)
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found");
            }

            return Ok(product);
        }
    }
}
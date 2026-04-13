using Microsoft.AspNetCore.Mvc;
using WebShopMercantec.Services;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Controllers;

/// <summary>
/// API контроллер для работы с продуктами и аксессуарами
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // === ПРОДУКТЫ ===

    /// <summary>
    /// Получить все доступные продукты
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await _productService.GetAvailableProductsAsync();
        return Ok(products);
    }

    /// <summary>
    /// Получить продукт по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        // Сервис сам бросит NotFoundException если не найдено
        // Middleware перехватит и вернет 404
        var product = await _productService.GetProductByIdAsync(id);
        return Ok(product);
    }

    /// <summary>
    /// Активировать продукт / изменить статус [Admin only]
    /// </summary>
    [HttpPut("{id}/activate")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ProductDto>> ActivateProduct(int id, [FromBody] int statusId)
    {
        var product = await _productService.ActivateProductAsync(id, statusId);
        return Ok(product);
    }

    /// <summary>
    /// Добавить новое устройство [Admin only]
    /// </summary>
    [HttpPost]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateDeviceDto dto)
    {
        var product = await _productService.CreateProductAsync(dto);
        return Ok(product);
    }

    /// <summary>
    /// Включить/выключить устройство в продаже (requestable) [Admin only]
    /// </summary>
    [HttpPut("{id}/requestable")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ProductDto>> SetRequestable(int id, [FromBody] bool requestable)
    {
        var product = await _productService.SetProductRequestableAsync(id, requestable);
        return Ok(product);
    }

    /// <summary>
    /// Получить продукты с пагинацией и фильтрами
    /// GET /api/products/paged?page=1&amp;pageSize=20&amp;categoryId=3&amp;search=laptop
    /// </summary>
    [HttpGet("paged")]
    public async Task<ActionResult> GetProductsPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? categoryId = null,
        [FromQuery] int? manufacturerId = null,
        [FromQuery] string? search = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null)
    {
        var (products, totalCount) = await _productService.GetProductsPagedAsync(
            page, pageSize, categoryId, manufacturerId, search, minPrice, maxPrice);

        return Ok(new
        {
            items = products,
            totalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }

    /// <summary>
    /// Получить продукты с пагинацией для админа (включает недоступные)
    /// GET /api/products/admin-paged?page=1&amp;pageSize=20
    /// </summary>
    [HttpGet("admin-paged")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult> GetAdminProductsPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
    {
        var (products, totalCount) = await _productService.GetAdminProductsPagedAsync(page, pageSize, search);

        return Ok(new
        {
            items = products,
            totalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }

    /// <summary>
    /// Поиск продуктов
    /// GET /api/products/search?q=laptop
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string q)
    {
        var products = await _productService.SearchProductsAsync(q);
        return Ok(products);
    }

    /// <summary>
    /// Получить продукты по категории
    /// GET /api/products/category/3
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetByCategory(int categoryId)
    {
        var products = await _productService.GetProductsByCategoryAsync(categoryId);
        return Ok(products);
    }

    /// <summary>
    /// Получить продукты по производителю
    /// GET /api/products/manufacturer/5
    /// </summary>
    [HttpGet("manufacturer/{manufacturerId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetByManufacturer(int manufacturerId)
    {
        var products = await _productService.GetProductsByManufacturerAsync(manufacturerId);
        return Ok(products);
    }

    // === АКСЕССУАРЫ ===

    /// <summary>
    /// Получить все доступные аксессуары
    /// GET /api/products/accessories
    /// </summary>
    [HttpGet("accessories")]
    public async Task<ActionResult<IEnumerable<AccessoryDto>>> GetAccessories()
    {
        var accessories = await _productService.GetAvailableAccessoriesAsync();
        return Ok(accessories);
    }

    /// <summary>
    /// Получить аксессуар по ID
    /// GET /api/products/accessories/5
    /// </summary>
    [HttpGet("accessories/{id}")]
    public async Task<ActionResult<AccessoryDto>> GetAccessory(int id)
    {
        var accessory = await _productService.GetAccessoryByIdAsync(id);
        return Ok(accessory);
    }

    /// <summary>
    /// Получить аксессуары с пагинацией
    /// GET /api/products/accessories/paged?page=1&amp;pageSize=20
    /// </summary>
    [HttpGet("accessories/paged")]
    public async Task<ActionResult> GetAccessoriesPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int? categoryId = null,
        [FromQuery] string? search = null)
    {
        var (accessories, totalCount) = await _productService.GetAccessoriesPagedAsync(
            page, pageSize, categoryId, search);

        return Ok(new
        {
            items = accessories,
            totalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }

    // === ПРОВЕРКИ ДОСТУПНОСТИ ===

    /// <summary>
    /// Проверить доступность продукта
    /// GET /api/products/5/available
    /// </summary>
    [HttpGet("{id}/available")]
    public async Task<ActionResult<bool>> IsProductAvailable(int id)
    {
        var isAvailable = await _productService.IsProductAvailableAsync(id);
        return Ok(new { productId = id, available = isAvailable });
    }

    /// <summary>
    /// Проверить доступность аксессуара
    /// GET /api/products/accessories/5/available?quantity=3
    /// </summary>
    [HttpGet("accessories/{id}/available")]
    public async Task<ActionResult<bool>> IsAccessoryAvailable(int id, [FromQuery] int quantity = 1)
    {
        var isAvailable = await _productService.IsAccessoryAvailableAsync(id, quantity);
        return Ok(new { accessoryId = id, quantity, available = isAvailable });
    }
}

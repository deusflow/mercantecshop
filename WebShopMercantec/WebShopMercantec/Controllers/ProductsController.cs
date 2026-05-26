using Microsoft.AspNetCore.Mvc;
using WebShopMercantec.Services;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Controllers;

// handles products and accessories
[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // --- PRODUCTS ---

    // get all available products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await _productService.GetAvailableProductsAsync();
        return Ok(products);
    }

    // get product by id
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        // 404 is handled by middleware
        var product = await _productService.GetProductByIdAsync(id);
        return Ok(product);
    }

    // admin: update product status/activation
    [HttpPut("{id}/activate")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ProductDto>> ActivateProduct(int id, [FromBody] int statusId)
    {
        var product = await _productService.ActivateProductAsync(id, statusId);
        return Ok(product);
    }

    // admin: add new device
    [HttpPost]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateDeviceDto dto)
    {
        var product = await _productService.CreateProductAsync(dto);
        return Ok(product);
    }

    // admin: toggle requestable status
    [HttpPut("{id}/requestable")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ProductDto>> SetRequestable(int id, [FromBody] bool requestable)
    {
        var product = await _productService.SetProductRequestableAsync(id, requestable);
        return Ok(product);
    }

    // admin: set purchase/sale price
    [HttpPut("{id}/price")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ProductDto>> SetPrice(int id, [FromBody] UpdateProductPriceDto dto)
    {
        var product = await _productService.SetProductPriceAsync(id, dto.Price);
        return Ok(product);
    }

    // get paged products with filters
    // GET /api/products/paged?page=1&amp;pageSize=20&amp;categoryId=3&amp;search=laptop
    
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

    // admin: get paged products including unavailable ones
    // GET /api/products/admin-paged?page=1&amp;pageSize=20
    
    [HttpGet("admin-paged")]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult> GetAdminProductsPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] int? categoryId = null,
        [FromQuery] bool? hasPrice = null)
    {
        var (products, totalCount) = await _productService.GetAdminProductsPagedAsync(page, pageSize, search, categoryId, hasPrice);

        return Ok(new
        {
            items = products,
            totalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }

    // search products by query string
    // GET /api/products/search?q=laptop
    
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string q)
    {
        var products = await _productService.SearchProductsAsync(q);
        return Ok(products);
    }

    // get products by category id
    // GET /api/products/category/3
    
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetByCategory(int categoryId)
    {
        var products = await _productService.GetProductsByCategoryAsync(categoryId);
        return Ok(products);
    }

    // get products by manufacturer id
    // GET /api/products/manufacturer/5
    
    [HttpGet("manufacturer/{manufacturerId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetByManufacturer(int manufacturerId)
    {
        var products = await _productService.GetProductsByManufacturerAsync(manufacturerId);
        return Ok(products);
    }

    // --- ACCESSORIES ---

    // get all available accessories
    [HttpGet("accessories")]
    public async Task<ActionResult<IEnumerable<AccessoryDto>>> GetAccessories()
    {
        var accessories = await _productService.GetAvailableAccessoriesAsync();
        return Ok(accessories);
    }

    // get accessory details
    [HttpGet("accessories/{id}")]
    public async Task<ActionResult<AccessoryDto>> GetAccessory(int id)
    {
        var accessory = await _productService.GetAccessoryByIdAsync(id);
        return Ok(accessory);
    }

    // get paged accessories
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

    // --- AVAILABILITY CHECKS ---

    // check if product asset is available
    [HttpGet("{id}/available")]
    public async Task<ActionResult<bool>> IsProductAvailable(int id)
    {
        var isAvailable = await _productService.IsProductAvailableAsync(id);
        return Ok(new { productId = id, available = isAvailable });
    }

    // check if accessory has enough stock
    [HttpGet("accessories/{id}/available")]
    public async Task<ActionResult<bool>> IsAccessoryAvailable(int id, [FromQuery] int quantity = 1)
    {
        var isAvailable = await _productService.IsAccessoryAvailableAsync(id, quantity);
        return Ok(new { accessoryId = id, quantity, available = isAvailable });
    }
}

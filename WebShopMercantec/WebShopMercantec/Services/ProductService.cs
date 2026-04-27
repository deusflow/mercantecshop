using WebShopMercantec.Repositories;
using WebShopMercantec.Exceptions;
using WebShopMercantec.Mapping;

namespace WebShopMercantec.Services;

// Сервис для работы с продуктами и аксессуарами
// Использует Repository Pattern с загрузкой связанных данных

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IUnitOfWork unitOfWork, ILogger<ProductService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // === ОСНОВНЫЕ МЕТОДЫ ===

    public async Task<IEnumerable<ProductDto>> GetAvailableProductsAsync()
    {
        _logger.LogInformation("Getting all available products");
        
        var assetsWithDetails = await _unitOfWork.Products.GetAvailableProductsWithDetailsAsync();
        var products = ProductMapping.MapFromDetailsList(assetsWithDetails).ToList();
        
        _logger.LogInformation("Found {Count} available products", products.Count);
        return products;
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        _logger.LogInformation("Getting product with ID: {ProductId}", id);
        
        var details = await _unitOfWork.Products.GetProductWithDetailsAsync((uint)id);
        
        if (details == null)
        {
            _logger.LogWarning("Product not found: {ProductId}", id);
            throw new NotFoundException("Product", id);
        }
        
        return ProductMapping.MapFromDetails(details);
    }

    // === ПАГИНАЦИЯ И ФИЛЬТРАЦИЯ ===

    public async Task<(IEnumerable<ProductDto> Products, int TotalCount)> GetProductsPagedAsync(
        int pageNumber, int pageSize, int? categoryId = null,
        int? manufacturerId = null, string? searchTerm = null,
        decimal? minPrice = null, decimal? maxPrice = null)
    {
        _logger.LogInformation(
            "Getting paged products: Page={Page}, Size={Size}, Category={Category}, Search={Search}",
            pageNumber, pageSize, categoryId, searchTerm);

        var (assetsWithDetails, totalCount) = await _unitOfWork.Products.GetProductsPagedWithDetailsAsync(
            pageNumber, pageSize, categoryId, manufacturerId, null, searchTerm,
            minPrice, maxPrice, availableOnly: true);

        var products = ProductMapping.MapFromDetailsList(assetsWithDetails).ToList();
        return (products, totalCount);
    }

    public async Task<(IEnumerable<ProductDto> Products, int TotalCount)> GetAdminProductsPagedAsync(
        int pageNumber, int pageSize, string? searchTerm = null, int? categoryId = null, bool? hasPrice = null)
    {
        _logger.LogInformation(
            "Getting admin paged products: Page={Page}, Size={Size}, Search={Search}, Category={CategoryId}, HasPrice={HasPrice}",
            pageNumber, pageSize, searchTerm, categoryId, hasPrice);

        var (assetsWithDetails, totalCount) = await _unitOfWork.Products.GetProductsPagedWithDetailsAsync(
            pageNumber, pageSize, categoryId, null, null, searchTerm, null, null, hasPrice, availableOnly: false);

        var products = ProductMapping.MapFromDetailsList(assetsWithDetails).ToList();
        return (products, totalCount);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        _logger.LogInformation("Getting products by category: {CategoryId}", categoryId);
        
        var assetsWithDetails = await _unitOfWork.Products.GetByCategoryWithDetailsAsync(categoryId);
        return ProductMapping.MapFromDetailsList(assetsWithDetails).ToList();
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByManufacturerAsync(int manufacturerId)
    {
        _logger.LogInformation("Getting products by manufacturer: {ManufacturerId}", manufacturerId);
        
        var assetsWithDetails = await _unitOfWork.Products.GetByManufacturerWithDetailsAsync(manufacturerId);
        return ProductMapping.MapFromDetailsList(assetsWithDetails).ToList();
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
    {
        _logger.LogInformation("Searching products: {SearchTerm}", searchTerm);
        
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            throw new BadRequestException("Search term cannot be empty");
        }
        
        var assetsWithDetails = await _unitOfWork.Products.SearchProductsWithDetailsAsync(searchTerm);
        return ProductMapping.MapFromDetailsList(assetsWithDetails).ToList();
    }

    public async Task<ProductDto> ActivateProductAsync(int productId, int statusId)
    {
        _logger.LogInformation("Activating product: {ProductId} to status: {StatusId}", productId, statusId);
        
        var asset = await _unitOfWork.Products.GetByIdAsync((uint)productId);
        if (asset == null || asset.DeletedAt != null)
            throw new NotFoundException("Product", productId);
            
        asset.StatusId = statusId;
        asset.UpdatedAt = DateTime.UtcNow;
        
        _unitOfWork.Products.Update(asset);
        await _unitOfWork.SaveChangesAsync();
        
        return await GetProductByIdAsync(productId) ?? throw new InvalidOperationException("Failed to reload product");
    }

    public async Task<ProductDto> SetProductRequestableAsync(int productId, bool requestable)
    {
        var asset = await _unitOfWork.Products.GetByIdAsync((uint)productId);
        if (asset == null || asset.DeletedAt != null)
            throw new NotFoundException("Product", productId);

        asset.Requestable = requestable ? (sbyte)1 : (sbyte)0;
        asset.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Products.Update(asset);
        await _unitOfWork.SaveChangesAsync();

        return await GetProductByIdAsync(productId) ?? throw new InvalidOperationException("Failed to reload product");
    }

    public async Task<ProductDto> SetProductPriceAsync(int productId, decimal price)
    {
        if (price < 0)
            throw new BadRequestException("Price cannot be negative");

        var asset = await _unitOfWork.Products.GetByIdAsync((uint)productId);
        if (asset == null || asset.DeletedAt != null)
            throw new NotFoundException("Product", productId);

        asset.PurchaseCost = decimal.Round(price, 2, MidpointRounding.AwayFromZero);
        asset.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Products.Update(asset);
        await _unitOfWork.SaveChangesAsync();

        return await GetProductByIdAsync(productId) ?? throw new InvalidOperationException("Failed to reload product");
    }

    public async Task<ProductDto> CreateProductAsync(CreateDeviceDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new BadRequestException("Device name is required");
        if (string.IsNullOrWhiteSpace(dto.AssetTag))
            throw new BadRequestException("Asset tag is required");

        var normalizedTag = dto.AssetTag.Trim();
        var duplicateTag = await _unitOfWork.Context.Assets.AnyAsync(a => a.DeletedAt == null && a.AssetTag == normalizedTag);
        if (duplicateTag)
            throw new BadRequestException($"Asset tag '{normalizedTag}' already exists");

        var asset = new Asset
        {
            Name = dto.Name.Trim(),
            AssetTag = normalizedTag,
            ModelId = dto.ModelId,
            StatusId = dto.StatusId,
            PurchaseCost = dto.PurchaseCost,
            Requestable = dto.Requestable ? (sbyte)1 : (sbyte)0,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Products.AddAsync(asset);
        await _unitOfWork.SaveChangesAsync();

        return await GetProductByIdAsync((int)asset.Id) ?? throw new InvalidOperationException("Failed to load created device");
    }

    // === АКСЕССУАРЫ ===

    public async Task<IEnumerable<AccessoryDto>> GetAvailableAccessoriesAsync()
    {
        _logger.LogInformation("Getting all available accessories");
        
        var accessories = await _unitOfWork.Accessories.GetAvailableAccessoriesAsync();
        
        var dtos = accessories.Select(ProductMapping.MapAccessoryToDto).ToList();
        
        _logger.LogInformation("Found {Count} available accessories", dtos.Count);
        return dtos;
    }

    public async Task<AccessoryDto?> GetAccessoryByIdAsync(int id)
    {
        _logger.LogInformation("Getting accessory with ID: {AccessoryId}", id);
        
        var accessory = await _unitOfWork.Accessories.GetByIdAsync((uint)id);
        
        if (accessory == null)
        {
            _logger.LogWarning("Accessory not found: {AccessoryId}", id);
            throw new NotFoundException("Accessory", id);
        }
        
        return ProductMapping.MapAccessoryToDto(accessory);
    }

    public async Task<(IEnumerable<AccessoryDto> Accessories, int TotalCount)> GetAccessoriesPagedAsync(
        int pageNumber, int pageSize, int? categoryId = null, string? searchTerm = null)
    {
        _logger.LogInformation(
            "Getting paged accessories: Page={Page}, Size={Size}, Category={Category}",
            pageNumber, pageSize, categoryId);

        var (accessories, totalCount) = await _unitOfWork.Accessories.GetAccessoriesPagedAsync(
            pageNumber, pageSize, categoryId, null, searchTerm, availableOnly: true);

        var dtos = accessories.Select(ProductMapping.MapAccessoryToDto).ToList();
        return (dtos, totalCount);
    }

    // === ПРОВЕРКИ ДОСТУПНОСТИ ===

    public async Task<bool> IsProductAvailableAsync(int productId)
    {
        return await _unitOfWork.Products.IsAvailableForCheckoutAsync((uint)productId);
    }

    public async Task<bool> IsAccessoryAvailableAsync(int accessoryId, int requestedQuantity = 1)
    {
        return await _unitOfWork.Accessories.IsAvailableAsync((uint)accessoryId, requestedQuantity);
    }
}

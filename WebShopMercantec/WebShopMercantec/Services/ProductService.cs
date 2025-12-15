using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;
using WebShopMercantec.Shared.DTOs;
using WebShopMercantec.Repositories;
using WebShopMercantec.Exceptions;

namespace WebShopMercantec.Services;

/// <summary>
/// Сервис для работы с продуктами и аксессуарами
/// Использует Repository Pattern и обрабатывает ошибки
/// </summary>
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
        
        var assets = await _unitOfWork.Products.GetAvailableProductsAsync();
        
        var products = new List<ProductDto>();
        foreach (var asset in assets)
        {
            products.Add(await MapAssetToDtoAsync(asset));
        }
        
        _logger.LogInformation("Found {Count} available products", products.Count);
        return products;
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        _logger.LogInformation("Getting product with ID: {ProductId}", id);
        
        var asset = await _unitOfWork.Products.GetByIdAsync((uint)id);
        
        if (asset == null)
        {
            _logger.LogWarning("Product not found: {ProductId}", id);
            throw new NotFoundException("Product", id);
        }
        
        return await MapAssetToDtoAsync(asset);
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

        var (assets, totalCount) = await _unitOfWork.Products.GetProductsPagedAsync(
            pageNumber, pageSize, categoryId, manufacturerId, null, searchTerm,
            minPrice, maxPrice, availableOnly: true);

        var products = new List<ProductDto>();
        foreach (var asset in assets)
        {
            products.Add(await MapAssetToDtoAsync(asset));
        }

        return (products, totalCount);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        _logger.LogInformation("Getting products by category: {CategoryId}", categoryId);
        
        var assets = await _unitOfWork.Products.GetByCategoryAsync(categoryId);
        
        var products = new List<ProductDto>();
        foreach (var asset in assets)
        {
            products.Add(await MapAssetToDtoAsync(asset));
        }
        
        return products;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByManufacturerAsync(int manufacturerId)
    {
        _logger.LogInformation("Getting products by manufacturer: {ManufacturerId}", manufacturerId);
        
        var assets = await _unitOfWork.Products.GetByManufacturerAsync(manufacturerId);
        
        var products = new List<ProductDto>();
        foreach (var asset in assets)
        {
            products.Add(await MapAssetToDtoAsync(asset));
        }
        
        return products;
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
    {
        _logger.LogInformation("Searching products: {SearchTerm}", searchTerm);
        
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            throw new BadRequestException("Search term cannot be empty");
        }
        
        var assets = await _unitOfWork.Products.SearchProductsAsync(searchTerm);
        
        var products = new List<ProductDto>();
        foreach (var asset in assets)
        {
            products.Add(await MapAssetToDtoAsync(asset));
        }
        
        return products;
    }

    // === АКСЕССУАРЫ ===

    public async Task<IEnumerable<AccessoryDto>> GetAvailableAccessoriesAsync()
    {
        _logger.LogInformation("Getting all available accessories");
        
        var accessories = await _unitOfWork.Accessories.GetAvailableAccessoriesAsync();
        
        var dtos = new List<AccessoryDto>();
        foreach (var accessory in accessories)
        {
            dtos.Add(await MapAccessoryToDtoAsync(accessory));
        }
        
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
        
        return await MapAccessoryToDtoAsync(accessory);
    }

    public async Task<(IEnumerable<AccessoryDto> Accessories, int TotalCount)> GetAccessoriesPagedAsync(
        int pageNumber, int pageSize, int? categoryId = null, string? searchTerm = null)
    {
        _logger.LogInformation(
            "Getting paged accessories: Page={Page}, Size={Size}, Category={Category}",
            pageNumber, pageSize, categoryId);

        var (accessories, totalCount) = await _unitOfWork.Accessories.GetAccessoriesPagedAsync(
            pageNumber, pageSize, categoryId, null, searchTerm, availableOnly: true);

        var dtos = new List<AccessoryDto>();
        foreach (var accessory in accessories)
        {
            dtos.Add(await MapAccessoryToDtoAsync(accessory));
        }

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

    // === МАППИНГ (PRIVATE МЕТОДЫ) ===

    private Task<ProductDto> MapAssetToDtoAsync(Asset asset)
    {
        // TODO: В будущем получать связанные данные через репозитории
        // (Model, Category, Manufacturer, StatusLabel)
        
        var dto = new ProductDto
        {
            Id = (int)asset.Id,
            Name = asset.Name ?? "Unknown Product",
            AssetTag = asset.AssetTag ?? "N/A",
            Image = asset.Image,
            ModelId = asset.ModelId,
            ModelName = null, // TODO: получить из Model
            Serial = asset.Serial,
            StatusId = asset.StatusId,
            StatusLabel = asset.StatusId.HasValue ? $"Status {asset.StatusId}" : "Unknown",
            CategoryName = "Unknown", // TODO: получить из Category через Model
            Notes = asset.Notes,
            PurchaseCost = asset.PurchaseCost,
            Price = asset.PurchaseCost ?? 0m,
            OrderNumber = asset.OrderNumber,
            ManufacturerId = null, // TODO: получить из Model
            ManufacturerName = null,
            ModelNumber = null,
            LocationId = asset.LocationId
        };
        
        return Task.FromResult(dto);
    }

    private Task<AccessoryDto> MapAccessoryToDtoAsync(Accessory accessory)
    {
        var dto = new AccessoryDto
        {
            Id = (int)accessory.Id,
            Name = accessory.Name ?? "Unknown Accessory",
            CategoryId = accessory.CategoryId,
            CategoryName = null, // TODO: получить из Category
            Qty = accessory.Qty,
            Requestable = accessory.Requestable,
            LocationId = accessory.LocationId,
            LocationName = null, // TODO: получить из Location
            PurchaseDate = accessory.PurchaseDate.HasValue 
                ? DateTime.Parse(accessory.PurchaseDate.Value.ToString("yyyy-MM-dd")) 
                : null,
            PurchaseCost = accessory.PurchaseCost,
            OrderNumber = accessory.OrderNumber,
            CompanyId = (int?)accessory.CompanyId,
            MinAmt = accessory.MinAmt,
            ManufacturerId = accessory.ManufacturerId,
            ManufacturerName = null, // TODO: получить из Manufacturer
            ModelNumber = accessory.ModelNumber,
            Image = accessory.Image,
            SupplierId = accessory.SupplierId,
            SupplierName = null, // TODO: получить из Supplier
            Notes = accessory.Notes,
            CreatedAt = accessory.CreatedAt,
            UpdatedAt = accessory.UpdatedAt
        };
        
        return Task.FromResult(dto);
    }
}


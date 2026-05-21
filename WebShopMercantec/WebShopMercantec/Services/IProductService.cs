using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Services;

// Product (asset) service interface

public interface IProductService
{
    // === CORE METHODS ===
    Task<IEnumerable<ProductDto>> GetAvailableProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    
    // === PAGINATION AND FILTERING ===
    Task<(IEnumerable<ProductDto> Products, int TotalCount)> GetProductsPagedAsync(
        int pageNumber, int pageSize, int? categoryId = null,
        int? manufacturerId = null, string? searchTerm = null,
        decimal? minPrice = null, decimal? maxPrice = null);
        
    Task<(IEnumerable<ProductDto> Products, int TotalCount)> GetAdminProductsPagedAsync(
        int pageNumber, int pageSize, string? searchTerm = null, int? categoryId = null, bool? hasPrice = null);
    
    Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<ProductDto>> GetProductsByManufacturerAsync(int manufacturerId);
    Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
    
    Task<ProductDto> ActivateProductAsync(int productId, int statusId);
    Task<ProductDto> SetProductRequestableAsync(int productId, bool requestable);
    Task<ProductDto> SetProductPriceAsync(int productId, decimal price);
    Task<ProductDto> CreateProductAsync(CreateDeviceDto dto);

    // === ACCESSORIES ===
    Task<IEnumerable<AccessoryDto>> GetAvailableAccessoriesAsync();
    Task<AccessoryDto?> GetAccessoryByIdAsync(int id);
    Task<(IEnumerable<AccessoryDto> Accessories, int TotalCount)> GetAccessoriesPagedAsync(
        int pageNumber, int pageSize, int? categoryId = null, string? searchTerm = null);
    
    // === VALIDATIONS ===
    Task<bool> IsProductAvailableAsync(int productId);
    Task<bool> IsAccessoryAvailableAsync(int accessoryId, int requestedQuantity = 1);
}

using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Services;

/// <summary>
/// Интерфейс сервиса для работы с продуктами (Assets)
/// </summary>
public interface IProductService
{
    // === ОСНОВНЫЕ МЕТОДЫ ===
    Task<IEnumerable<ProductDto>> GetAvailableProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    
    // === ПАГИНАЦИЯ И ФИЛЬТРАЦИЯ ===
    Task<(IEnumerable<ProductDto> Products, int TotalCount)> GetProductsPagedAsync(
        int pageNumber, int pageSize, int? categoryId = null,
        int? manufacturerId = null, string? searchTerm = null,
        decimal? minPrice = null, decimal? maxPrice = null);
    
    Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<ProductDto>> GetProductsByManufacturerAsync(int manufacturerId);
    Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
    
    // === АКСЕССУАРЫ ===
    Task<IEnumerable<AccessoryDto>> GetAvailableAccessoriesAsync();
    Task<AccessoryDto?> GetAccessoryByIdAsync(int id);
    Task<(IEnumerable<AccessoryDto> Accessories, int TotalCount)> GetAccessoriesPagedAsync(
        int pageNumber, int pageSize, int? categoryId = null, string? searchTerm = null);
    
    // === ПРОВЕРКИ ===
    Task<bool> IsProductAvailableAsync(int productId);
    Task<bool> IsAccessoryAvailableAsync(int accessoryId, int requestedQuantity = 1);
}


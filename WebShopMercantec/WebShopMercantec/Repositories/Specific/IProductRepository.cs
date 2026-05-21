using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

public interface IProductRepository : IRepository<Asset>
{
    
    Task<IEnumerable<Asset>> GetAvailableProductsAsync();
    
    
    
    Task<IEnumerable<Asset>> GetByCategoryAsync(int categoryId);
    
    
    Task<IEnumerable<Asset>> GetByModelIdAsync(int modelId);
    
    
    Task<IEnumerable<Asset>> GetByManufacturerAsync(int manufacturerId);
    
    
    Task<IEnumerable<Asset>> GetByStatusIdAsync(int statusId);
    
    
    Task<IEnumerable<Asset>> GetByLocationIdAsync(int locationId);
    
    
    Task<IEnumerable<Asset>> GetAssignedToUserAsync(int userId);
    
    
    Task<IEnumerable<Asset>> SearchProductsAsync(string searchTerm);
    
    
    Task<(IEnumerable<Asset> Products, int TotalCount)> GetProductsPagedAsync(
        int pageNumber,
        int pageSize,
        int? categoryId = null,
        int? manufacturerId = null,
        int? statusId = null,
        string? searchTerm = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? availableOnly = true);
    
    
    Task<bool> IsAvailableForCheckoutAsync(uint assetId);
    
    
    Task<Asset?> GetByAssetTagAsync(string assetTag);
    
    
    Task<IEnumerable<Asset>> GetArchivedProductsAsync();
    
    
    Task<IEnumerable<Asset>> GetProductsRequiringMaintenanceAsync();
    
    // === METHODS WITH RELATED DATA (AssetWithDetails) ===
    
    
    Task<AssetWithDetails?> GetProductWithDetailsAsync(uint id);
    
    
    Task<IEnumerable<AssetWithDetails>> GetAvailableProductsWithDetailsAsync();
    
    
    Task<(IEnumerable<AssetWithDetails> Products, int TotalCount)> GetProductsPagedWithDetailsAsync(
        int pageNumber,
        int pageSize,
        int? categoryId = null,
        int? manufacturerId = null,
        int? statusId = null,
        string? searchTerm = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? hasPrice = null,
        bool? availableOnly = true);
    
    
    Task<IEnumerable<AssetWithDetails>> SearchProductsWithDetailsAsync(string searchTerm);
    
    
    Task<IEnumerable<AssetWithDetails>> GetByCategoryWithDetailsAsync(int categoryId);
    
    
    Task<IEnumerable<AssetWithDetails>> GetByManufacturerWithDetailsAsync(int manufacturerId);
}
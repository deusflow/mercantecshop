using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

public interface IAccessoryRepository : IRepository<Accessory>
{
    
    Task<IEnumerable<Accessory>> GetAvailableAccessoriesAsync();
    
    
    Task<IEnumerable<Accessory>> GetByCategoryIdAsync(int categoryId);
    
    
    Task<IEnumerable<Accessory>> GetByManufacturerIdAsync(int manufacturerId);
    
    
    Task<IEnumerable<Accessory>> GetByLocationIdAsync(int locationId);
    
    
    Task<IEnumerable<Accessory>> GetByCompanyIdAsync(uint companyId);
    
    
    Task<IEnumerable<Accessory>> SearchAccessoriesAsync(string searchTerm);
    
    
    Task<(IEnumerable<Accessory> Accessories, int TotalCount)> GetAccessoriesPagedAsync(
        int pageNumber,
        int pageSize,
        int? categoryId = null,
        int? manufacturerId = null,
        string? searchTerm = null,
        bool? availableOnly = true);
    
    
    Task<bool> IsAvailableAsync(uint accessoryId, int requestedQuantity = 1);
    
    
    Task<int> GetAvailableQuantityAsync(uint accessoryId);
    
    
    Task<IEnumerable<Accessory>> GetLowStockAccessoriesAsync();
    
    
    Task<IEnumerable<Accessory>> GetOutOfStockAccessoriesAsync();
    
    
    Task<bool> UpdateQuantityAsync(uint accessoryId, int quantityChange);
}
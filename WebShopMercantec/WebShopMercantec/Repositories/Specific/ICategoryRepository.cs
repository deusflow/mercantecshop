using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

public interface ICategoryRepository : IRepository<Category>
{
    
    Task<IEnumerable<Category>> GetAllActiveCategoriesAsync();
    
    
    Task<Category?> GetActiveCategoryByIdAsync(uint id);
    
    
    Task<IEnumerable<Category>> GetCategoriesByTypeAsync(string categoryType);
    
    
    Task<int> GetItemsCountAsync(uint categoryId);
    
    
    Task<Dictionary<uint, int>> GetAllItemsCountsBatchAsync();
}


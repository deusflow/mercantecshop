using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Services;

// Category service interface
// Defines business logic for category management

public interface ICategoryService
{
    // Get all active categories
    
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();

    // Get categories for catalog (asset) with available quantity
    
    Task<IEnumerable<CategoryDto>> GetCatalogCategoriesAsync(bool includeHidden = false);

    // Toggle category visibility in the catalog
    
    Task SetCategoryVisibilityAsync(int categoryId, bool visible);

    // Get category by ID
    
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    
    // Get categories by type (asset, accessory, consumable, component)
    
    Task<IEnumerable<CategoryDto>> GetCategoriesByTypeAsync(string categoryType);
    
    // Create a new category
    
    Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto);
    
    // Update an existing category
    
    Task<CategoryDto> UpdateCategoryAsync(int id, CategoryDto categoryDto);
    
    // Delete a category (soft delete)
    
    Task DeleteCategoryAsync(int id);
}

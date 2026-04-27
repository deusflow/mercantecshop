using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Services;

// Интерфейс сервиса для работы с категориями
// Определяет бизнес-логику для управления категориями

public interface ICategoryService
{
    // Получить все активные категории
    
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();

    // Получить категории для каталога (asset) с доступным количеством.
    
    Task<IEnumerable<CategoryDto>> GetCatalogCategoriesAsync(bool includeHidden = false);

    // Изменить видимость категории в каталоге.
    
    Task SetCategoryVisibilityAsync(int categoryId, bool visible);

    // Получить категорию по ID
    
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    
    // Получить категории по типу (asset, accessory, consumable, component)
    
    Task<IEnumerable<CategoryDto>> GetCategoriesByTypeAsync(string categoryType);
    
    // Создать новую категорию
    
    Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto);
    
    // Обновить существующую категорию
    
    Task<CategoryDto> UpdateCategoryAsync(int id, CategoryDto categoryDto);
    
    // Удалить категорию (soft delete)
    
    Task DeleteCategoryAsync(int id);
}

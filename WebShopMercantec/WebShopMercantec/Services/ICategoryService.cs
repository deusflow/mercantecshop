using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Services;

/// <summary>
/// Интерфейс сервиса для работы с категориями
/// Определяет бизнес-логику для управления категориями
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Получить все активные категории
    /// </summary>
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    
    /// <summary>
    /// Получить категорию по ID
    /// </summary>
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    
    /// <summary>
    /// Получить категории по типу (asset, accessory, consumable, component)
    /// </summary>
    Task<IEnumerable<CategoryDto>> GetCategoriesByTypeAsync(string categoryType);
    
    /// <summary>
    /// Создать новую категорию
    /// </summary>
    Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto);
    
    /// <summary>
    /// Обновить существующую категорию
    /// </summary>
    Task<CategoryDto> UpdateCategoryAsync(int id, CategoryDto categoryDto);
    
    /// <summary>
    /// Удалить категорию (soft delete)
    /// </summary>
    Task DeleteCategoryAsync(int id);
}


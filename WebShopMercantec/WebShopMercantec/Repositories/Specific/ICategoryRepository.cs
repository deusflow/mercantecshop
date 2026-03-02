using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

/// <summary>
/// Интерфейс репозитория для работы с категориями
/// Расширяет базовый IRepository дополнительными методами для Category
/// </summary>
public interface ICategoryRepository : IRepository<Category>
{
    /// <summary>
    /// Получить все активные (не удалённые) категории с сортировкой по имени
    /// </summary>
    Task<IEnumerable<Category>> GetAllActiveCategoriesAsync();
    
    /// <summary>
    /// Получить категорию по ID (только активные)
    /// </summary>
    Task<Category?> GetActiveCategoryByIdAsync(uint id);
    
    /// <summary>
    /// Получить категории определённого типа (asset, accessory, consumable, component)
    /// </summary>
    Task<IEnumerable<Category>> GetCategoriesByTypeAsync(string categoryType);
    
    /// <summary>
    /// Подсчитать количество элементов в категории
    /// </summary>
    Task<int> GetItemsCountAsync(uint categoryId);
    
    /// <summary>
    /// Получить количество элементов для ВСЕХ категорий одним запросом (batch)
    /// Возвращает Dictionary categoryId → count
    /// Решает проблему N+1 запросов при вызове GetItemsCountAsync в цикле
    /// </summary>
    Task<Dictionary<uint, int>> GetAllItemsCountsBatchAsync();
}


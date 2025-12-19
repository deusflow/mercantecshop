using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

/// <summary>
/// Репозиторий для работы с категориями
/// Наследует базовый Repository и добавляет специфичные для Category методы
/// </summary>
public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(SnipeItContext context) : base(context)
    {
    }

    /// <summary>
    /// Получить все активные (не удалённые) категории с сортировкой по имени
    /// </summary>
    public async Task<IEnumerable<Category>> GetAllActiveCategoriesAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.DeletedAt == null)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Получить категорию по ID (только активные)
    /// </summary>
    public async Task<Category?> GetActiveCategoryByIdAsync(uint id)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.Id == id && c.DeletedAt == null)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Получить категории определённого типа (asset, accessory, consumable, component)
    /// </summary>
    public async Task<IEnumerable<Category>> GetCategoriesByTypeAsync(string categoryType)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(c => c.DeletedAt == null && c.CategoryType == categoryType)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Подсчитать количество элементов в категории
    /// Считает только активные (не удалённые) элементы
    /// </summary>
    public async Task<int> GetItemsCountAsync(uint categoryId)
    {
        // Подсчитываем Assets (продукты) в этой категории через Model
        var assetsCount = await (
            from asset in _context.Assets
            join model in _context.Models on asset.ModelId equals (int?)model.Id
            where model.CategoryId == (int)categoryId 
                  && asset.DeletedAt == null 
                  && model.DeletedAt == null
            select asset
        ).CountAsync();

        // Подсчитываем Accessories в этой категории
        var accessoriesCount = await _context.Accessories
            .Where(a => a.CategoryId == categoryId && a.DeletedAt == null)
            .CountAsync();

        return assetsCount + accessoriesCount;
    }
}


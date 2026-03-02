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

    /// <summary>
    /// Получить количество элементов для всех категорий одним запросом
    /// Вместо N+1 запросов — всего 2 запроса (assets + accessories)
    /// </summary>
    public async Task<Dictionary<uint, int>> GetAllItemsCountsBatchAsync()
    {
        // 1. Подсчитываем Assets по категориям (через Model -> CategoryId)
        var assetCounts = await (
            from model in _context.Models.AsNoTracking()
            where model.DeletedAt == null && model.CategoryId.HasValue
            join asset in _context.Assets.AsNoTracking().Where(a => a.DeletedAt == null)
                on (int?)model.Id equals asset.ModelId
            group asset by (uint)model.CategoryId!.Value into g
            select new { CategoryId = g.Key, Count = g.Count() }
        ).ToDictionaryAsync(x => x.CategoryId, x => x.Count);

        // 2. Подсчитываем Accessories по категориям
        var accessoryCounts = await _context.Accessories
            .AsNoTracking()
            .Where(a => a.DeletedAt == null && a.CategoryId.HasValue)
            .GroupBy(a => (uint)a.CategoryId!.Value)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CategoryId, x => x.Count);

        // 3. Мержим результаты
        var result = new Dictionary<uint, int>(assetCounts);
        foreach (var (categoryId, count) in accessoryCounts)
        {
            if (result.ContainsKey(categoryId))
                result[categoryId] += count;
            else
                result[categoryId] = count;
        }

        return result;
    }
}


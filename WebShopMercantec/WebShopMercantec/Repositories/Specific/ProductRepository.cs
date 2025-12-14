using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

/// <summary>
/// Product Repository Implementation
/// Реализация репозитория для работы с продуктами (Assets)
/// </summary>
public class ProductRepository : Repository<Asset>, IProductRepository
{
    public ProductRepository(SnipeItContext context) : base(context)
    {
    }

    /// <summary>
    /// Получить доступные для заказа продукты
    /// Условия:
    /// - StatusId = 1 или 2 (Ready to Deploy / Deployable)
    /// - Не архивировано
    /// - Не удалено
    /// - Можно запросить (Requestable = 1)
    /// - Не назначено никому (AssignedTo = null)
    /// </summary>
    public async Task<IEnumerable<Asset>> GetAvailableProductsAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => 
                (a.StatusId == 1 || a.StatusId == 2) && // Ready to Deploy
                (a.Archived == false || a.Archived == null) &&
                a.DeletedAt == null &&
                a.Requestable == 1 &&
                a.AssignedTo == null) // Не назначено пользователю
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Получить продукты по категории
    /// Категория связана через Model -> CategoryId
    /// </summary>
    public async Task<IEnumerable<Asset>> GetByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => 
                a.ModelId.HasValue &&
                _context.Models.Any(m => m.Id == a.ModelId && m.CategoryId == categoryId) &&
                a.DeletedAt == null)
            .ToListAsync();
    }

    /// <summary>
    /// Получить продукты по модели
    /// </summary>
    public async Task<IEnumerable<Asset>> GetByModelIdAsync(int modelId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.ModelId == modelId && a.DeletedAt == null)
            .ToListAsync();
    }

    /// <summary>
    /// Получить продукты по производителю
    /// Производитель связан: Asset -> Model -> ManufacturerId
    /// </summary>
    public async Task<IEnumerable<Asset>> GetByManufacturerAsync(int manufacturerId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => 
                a.ModelId.HasValue &&
                _context.Models.Any(m => m.Id == a.ModelId && m.ManufacturerId == manufacturerId) &&
                a.DeletedAt == null)
            .ToListAsync();
    }

    /// <summary>
    /// Получить продукты по статусу
    /// </summary>
    public async Task<IEnumerable<Asset>> GetByStatusIdAsync(int statusId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.StatusId == statusId && a.DeletedAt == null)
            .ToListAsync();
    }

    /// <summary>
    /// Получить продукты по локации
    /// </summary>
    public async Task<IEnumerable<Asset>> GetByLocationIdAsync(int locationId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.LocationId == locationId && a.DeletedAt == null)
            .ToListAsync();
    }

    /// <summary>
    /// Получить продукты, назначенные конкретному пользователю
    /// </summary>
    public async Task<IEnumerable<Asset>> GetAssignedToUserAsync(int userId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => 
                a.AssignedTo == userId && 
                a.AssignedType == "App\\Models\\User" && // Проверяем тип назначения
                a.DeletedAt == null)
            .ToListAsync();
    }

    /// <summary>
    /// Поиск продуктов по названию, asset tag или серийному номеру
    /// </summary>
    public async Task<IEnumerable<Asset>> SearchProductsAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.DeletedAt == null && (
                (a.Name != null && a.Name.ToLower().Contains(term)) ||
                (a.AssetTag != null && a.AssetTag.ToLower().Contains(term)) ||
                (a.Serial != null && a.Serial.ToLower().Contains(term))
            ))
            .ToListAsync();
    }

    /// <summary>
    /// Получить продукты с пагинацией и множественными фильтрами
    /// ЭТО ОСНОВНОЙ МЕТОД ДЛЯ КАТАЛОГА МАГАЗИНА!
    /// </summary>
    public async Task<(IEnumerable<Asset> Products, int TotalCount)> GetProductsPagedAsync(
        int pageNumber, 
        int pageSize, 
        int? categoryId = null, 
        int? manufacturerId = null, 
        int? statusId = null, 
        string? searchTerm = null, 
        decimal? minPrice = null, 
        decimal? maxPrice = null, 
        bool? availableOnly = true)
    {
        // Базовый запрос
        var query = _dbSet.AsNoTracking().Where(a => a.DeletedAt == null);
        
        // Фильтр: только доступные для заказа
        if (availableOnly == true)
        {
            query = query.Where(a =>
                (a.StatusId == 1 || a.StatusId == 2) &&
                (a.Archived == false || a.Archived == null) &&
                a.Requestable == 1 &&
                a.AssignedTo == null);
        }
        
        // Фильтр по категории (через Model)
        if (categoryId.HasValue)
        {
            query = query.Where(a =>
                a.ModelId.HasValue &&
                _context.Models.Any(m => m.Id == a.ModelId && m.CategoryId == categoryId));
        }
        
        // Фильтр по производителю (через Model)
        if (manufacturerId.HasValue)
        {
            query = query.Where(a =>
                a.ModelId.HasValue &&
                _context.Models.Any(m => m.Id == a.ModelId && m.ManufacturerId == manufacturerId));
        }
        
        // Фильтр по статусу
        if (statusId.HasValue)
        {
            query = query.Where(a => a.StatusId == statusId);
        }
        
        // Фильтр по поиску
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(a =>
                (a.Name != null && a.Name.ToLower().Contains(term)) ||
                (a.AssetTag != null && a.AssetTag.ToLower().Contains(term)) ||
                (a.Serial != null && a.Serial.ToLower().Contains(term))
            );
        }
        
        // Фильтр по цене
        if (minPrice.HasValue)
        {
            query = query.Where(a => a.PurchaseCost >= minPrice.Value);
        }
        
        if (maxPrice.HasValue)
        {
            query = query.Where(a => a.PurchaseCost <= maxPrice.Value);
        }
        
        // Считаем общее количество
        var totalCount = await query.CountAsync();
        
        // Получаем страницу с сортировкой
        var products = await query
            .OrderBy(a => a.Name)
            .ThenBy(a => a.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (products, totalCount);
    }

    /// <summary>
    /// Проверить, доступен ли продукт для заказа
    /// </summary>
    public async Task<bool> IsAvailableForCheckoutAsync(uint assetId)
    {
        return await _dbSet
            .AnyAsync(a =>
                a.Id == assetId &&
                (a.StatusId == 1 || a.StatusId == 2) &&
                (a.Archived == false || a.Archived == null) &&
                a.DeletedAt == null &&
                a.Requestable == 1 &&
                a.AssignedTo == null);
    }

    /// <summary>
    /// Получить продукт по Asset Tag
    /// Asset Tag - это уникальный идентификатор (как штрих-код)
    /// </summary>
    public async Task<Asset?> GetByAssetTagAsync(string assetTag)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(a => 
                a.AssetTag != null && 
                a.AssetTag.ToLower() == assetTag.ToLower() &&
                a.DeletedAt == null);
    }

    /// <summary>
    /// Получить архивированные продукты
    /// </summary>
    public async Task<IEnumerable<Asset>> GetArchivedProductsAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.Archived == true && a.DeletedAt == null)
            .ToListAsync();
    }

    /// <summary>
    /// Получить продукты, требующие обслуживания
    /// Проверяем NextAuditDate - если дата прошла, нужен аудит
    /// </summary>
    public async Task<IEnumerable<Asset>> GetProductsRequiringMaintenanceAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        
        return await _dbSet
            .AsNoTracking()
            .Where(a => 
                a.DeletedAt == null &&
                a.NextAuditDate.HasValue &&
                a.NextAuditDate <= today)
            .ToListAsync();
    }
}
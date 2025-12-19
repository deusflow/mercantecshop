using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

/// <summary>
/// Accessory Repository Implementation
/// Реализация репозитория для работы с аксессуарами
/// </summary>
public class AccessoryRepository : Repository<Accessory>, IAccessoryRepository
{
    public AccessoryRepository(SnipeItContext context) : base(context)
    {
    }

    /// <summary>
    /// Получить доступные аксессуары
    /// Условия:
    /// - Requestable = true (можно запросить)
    /// - Qty > 0 (есть в наличии)
    /// - DeletedAt = null (не удалено)
    /// </summary>
    public async Task<IEnumerable<Accessory>> GetAvailableAccessoriesAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a =>
                a.Requestable == true &&
                a.Qty > 0 &&
                a.DeletedAt == null)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Получить аксессуары по категории
    /// </summary>
    public async Task<IEnumerable<Accessory>> GetByCategoryIdAsync(int categoryId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.CategoryId == categoryId && a.DeletedAt == null)
            .ToListAsync();
    }

    /// <summary>
    /// Получить аксессуары по производителю
    /// </summary>
    public async Task<IEnumerable<Accessory>> GetByManufacturerIdAsync(int manufacturerId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.ManufacturerId == manufacturerId && a.DeletedAt == null)
            .ToListAsync();
    }

    /// <summary>
    /// Получить аксессуары по локации
    /// </summary>
    public async Task<IEnumerable<Accessory>> GetByLocationIdAsync(int locationId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.LocationId == locationId && a.DeletedAt == null)
            .ToListAsync();
    }

    /// <summary>
    /// Получить аксессуары по компании
    /// </summary>
    public async Task<IEnumerable<Accessory>> GetByCompanyIdAsync(uint companyId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.CompanyId == companyId && a.DeletedAt == null)
            .ToListAsync();
    }

    /// <summary>
    /// Поиск аксессуаров по названию, номеру модели, заказу
    /// </summary>
    public async Task<IEnumerable<Accessory>> SearchAccessoriesAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.DeletedAt == null && (
                (a.Name != null && a.Name.ToLower().Contains(term)) ||
                (a.ModelNumber != null && a.ModelNumber.ToLower().Contains(term)) ||
                (a.OrderNumber != null && a.OrderNumber.ToLower().Contains(term))
            ))
            .ToListAsync();
    }

    /// <summary>
    /// Получить аксессуары с пагинацией и фильтрами
    /// ОСНОВНОЙ МЕТОД ДЛЯ КАТАЛОГА АКСЕССУАРОВ
    /// </summary>
    public async Task<(IEnumerable<Accessory> Accessories, int TotalCount)> GetAccessoriesPagedAsync(
        int pageNumber, 
        int pageSize, 
        int? categoryId = null, 
        int? manufacturerId = null, 
        string? searchTerm = null, 
        bool? availableOnly = true)
    {
        // Базовый запрос
        var query = _dbSet.AsNoTracking().Where(a => a.DeletedAt == null);
        
        // Фильтр: только доступные
        if (availableOnly == true)
        {
            query = query.Where(a => a.Requestable == true && a.Qty > 0);
        }
        
        // Фильтр по категории
        if (categoryId.HasValue)
        {
            query = query.Where(a => a.CategoryId == categoryId);
        }
        
        // Фильтр по производителю
        if (manufacturerId.HasValue)
        {
            query = query.Where(a => a.ManufacturerId == manufacturerId);
        }
        
        // Фильтр по поиску
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(a =>
                (a.Name != null && a.Name.ToLower().Contains(term)) ||
                (a.ModelNumber != null && a.ModelNumber.ToLower().Contains(term)) ||
                (a.OrderNumber != null && a.OrderNumber.ToLower().Contains(term))
            );
        }
        
        // Считаем общее количество
        var totalCount = await query.CountAsync();
        
        // Получаем страницу
        var accessories = await query
            .OrderBy(a => a.Name)
            .ThenBy(a => a.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (accessories, totalCount);
    }

    /// <summary>
    /// Проверить доступность аксессуара
    /// Проверяет, можно ли заказать указанное количество
    /// </summary>
    public async Task<bool> IsAvailableAsync(uint accessoryId, int requestedQuantity = 1)
    {
        var accessory = await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == accessoryId);
        
        if (accessory == null || accessory.DeletedAt != null)
            return false;
        
        return accessory.Requestable == true && accessory.Qty >= requestedQuantity;
    }

    /// <summary>
    /// Получить доступное количество аксессуара
    /// </summary>
    public async Task<int> GetAvailableQuantityAsync(uint accessoryId)
    {
        var accessory = await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == accessoryId && a.DeletedAt == null);
        
        return accessory?.Qty ?? 0;
    }

    /// <summary>
    /// Получить аксессуары с низким запасом
    /// Qty меньше или равно MinAmt (если MinAmt задан)
    /// Или Qty меньше 5 (если MinAmt не задан)
    /// </summary>
    public async Task<IEnumerable<Accessory>> GetLowStockAccessoriesAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a =>
                a.DeletedAt == null &&
                a.Qty > 0 && // Не полностью закончились
                (
                    (a.MinAmt.HasValue && a.Qty <= a.MinAmt.Value) ||
                    (!a.MinAmt.HasValue && a.Qty < 5) // Default threshold
                ))
            .OrderBy(a => a.Qty) // Сортируем по количеству (меньше первыми)
            .ToListAsync();
    }

    /// <summary>
    /// Получить аксессуары без запасов
    /// Qty = 0
    /// </summary>
    public async Task<IEnumerable<Accessory>> GetOutOfStockAccessoriesAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.DeletedAt == null && a.Qty == 0)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Обновить количество аксессуара
    /// quantityChange может быть положительным (добавление) или отрицательным (вычитание)
    /// ВАЖНО: Не вызывает SaveChanges! Используйте через UnitOfWork
    /// </summary>
    public async Task<bool> UpdateQuantityAsync(uint accessoryId, int quantityChange)
    {
        var accessory = await _dbSet.FindAsync(accessoryId);
        
        if (accessory == null || accessory.DeletedAt != null)
            return false;
        
        // Проверяем, что не уходим в минус
        var newQuantity = accessory.Qty + quantityChange;
        if (newQuantity < 0)
            return false;
        
        accessory.Qty = newQuantity;
        accessory.UpdatedAt = DateTime.UtcNow;
        
        // Update вызовется автоматически через EF Core change tracking
        return true;
    }

    // === МЕТОДЫ ДЛЯ ПОЛУЧЕНИЯ ENRICHED ДАННЫХ (СО СВЯЗЯМИ) ===

    /// <summary>
    /// Получить доступные аксессуары со всеми связанными данными
    /// </summary>
    public async Task<IEnumerable<EnrichedAccessory>> GetAvailableAccessoriesEnrichedAsync()
    {
        var query = from accessory in _context.Accessories
                    where accessory.DeletedAt == null &&
                          accessory.Requestable &&
                          accessory.Qty > 0
                    // LEFT JOIN с Category
                    join category in _context.Categories on accessory.CategoryId equals (int?)category.Id into categoryGroup
                    from category in categoryGroup.DefaultIfEmpty()
                    // LEFT JOIN с Manufacturer
                    join manufacturer in _context.Manufacturers on accessory.ManufacturerId equals (int?)manufacturer.Id into mfgGroup
                    from manufacturer in mfgGroup.DefaultIfEmpty()
                    // LEFT JOIN с Supplier
                    join supplier in _context.Suppliers on accessory.SupplierId equals (int?)supplier.Id into supplierGroup
                    from supplier in supplierGroup.DefaultIfEmpty()
                    // LEFT JOIN с Location
                    join location in _context.Locations on accessory.LocationId equals (int?)location.Id into locationGroup
                    from location in locationGroup.DefaultIfEmpty()
                    select new EnrichedAccessory
                    {
                        Accessory = accessory,
                        Category = category,
                        Manufacturer = manufacturer,
                        Supplier = supplier,
                        Location = location
                    };

        return await query.AsNoTracking().OrderBy(e => e.Accessory.Name).ToListAsync();
    }

    /// <summary>
    /// Получить аксессуар по ID со всеми связанными данными
    /// </summary>
    public async Task<EnrichedAccessory?> GetEnrichedAccessoryByIdAsync(uint id)
    {
        var query = from accessory in _context.Accessories
                    where accessory.Id == id && accessory.DeletedAt == null
                    join category in _context.Categories on accessory.CategoryId equals (int?)category.Id into categoryGroup
                    from category in categoryGroup.DefaultIfEmpty()
                    join manufacturer in _context.Manufacturers on accessory.ManufacturerId equals (int?)manufacturer.Id into mfgGroup
                    from manufacturer in mfgGroup.DefaultIfEmpty()
                    join supplier in _context.Suppliers on accessory.SupplierId equals (int?)supplier.Id into supplierGroup
                    from supplier in supplierGroup.DefaultIfEmpty()
                    join location in _context.Locations on accessory.LocationId equals (int?)location.Id into locationGroup
                    from location in locationGroup.DefaultIfEmpty()
                    select new EnrichedAccessory
                    {
                        Accessory = accessory,
                        Category = category,
                        Manufacturer = manufacturer,
                        Supplier = supplier,
                        Location = location
                    };

        return await query.AsNoTracking().FirstOrDefaultAsync();
    }

    /// <summary>
    /// Получить аксессуары с пагинацией СО СВЯЗЯМИ
    /// </summary>
    public async Task<(IEnumerable<EnrichedAccessory> Accessories, int TotalCount)> GetAccessoriesPagedEnrichedAsync(
        int pageNumber,
        int pageSize,
        int? categoryId = null,
        int? manufacturerId = null,
        string? searchTerm = null,
        bool? availableOnly = true)
    {
        var query = from accessory in _context.Accessories
                    join category in _context.Categories on accessory.CategoryId equals (int?)category.Id into categoryGroup
                    from category in categoryGroup.DefaultIfEmpty()
                    join manufacturer in _context.Manufacturers on accessory.ManufacturerId equals (int?)manufacturer.Id into mfgGroup
                    from manufacturer in mfgGroup.DefaultIfEmpty()
                    join supplier in _context.Suppliers on accessory.SupplierId equals (int?)supplier.Id into supplierGroup
                    from supplier in supplierGroup.DefaultIfEmpty()
                    join location in _context.Locations on accessory.LocationId equals (int?)location.Id into locationGroup
                    from location in locationGroup.DefaultIfEmpty()
                    where accessory.DeletedAt == null
                    select new 
                    { 
                        accessory, 
                        category, 
                        manufacturer, 
                        supplier, 
                        location 
                    };

        // Фильтры
        if (availableOnly == true)
        {
            query = query.Where(x => x.accessory.Requestable && x.accessory.Qty > 0);
        }

        if (categoryId.HasValue)
            query = query.Where(x => x.accessory.CategoryId == (uint)categoryId);

        if (manufacturerId.HasValue)
            query = query.Where(x => x.accessory.ManufacturerId == manufacturerId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var search = searchTerm.ToLower();
            query = query.Where(x => 
                (x.accessory.Name != null && x.accessory.Name.ToLower().Contains(search)) ||
                (x.accessory.ModelNumber != null && x.accessory.ModelNumber.ToLower().Contains(search)));
        }

        // Подсчет общего количества
        var totalCount = await query.CountAsync();

        // Пагинация
        var results = await query
            .OrderBy(x => x.accessory.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new EnrichedAccessory
            {
                Accessory = x.accessory,
                Category = x.category,
                Manufacturer = x.manufacturer,
                Supplier = x.supplier,
                Location = x.location
            })
            .AsNoTracking()
            .ToListAsync();

        return (results, totalCount);
    }
}


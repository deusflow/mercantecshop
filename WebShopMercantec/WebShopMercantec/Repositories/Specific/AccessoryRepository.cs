using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

public class AccessoryRepository : Repository<Accessory>, IAccessoryRepository
{
    private const int DefaultLowStockThreshold = 5;

    public AccessoryRepository(SnipeItContext context) : base(context)
    {
    }

    
    public async Task<IEnumerable<Accessory>> GetAvailableAccessoriesAsync()
    {
        return await ApplyAvailableAccessoryFilter(_dbSet.AsNoTracking())
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    
    public async Task<IEnumerable<Accessory>> GetByCategoryIdAsync(int categoryId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.CategoryId == categoryId && a.DeletedAt == null)
            .ToListAsync();
    }

    
    public async Task<IEnumerable<Accessory>> GetByManufacturerIdAsync(int manufacturerId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.ManufacturerId == manufacturerId && a.DeletedAt == null)
            .ToListAsync();
    }

    
    public async Task<IEnumerable<Accessory>> GetByLocationIdAsync(int locationId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.LocationId == locationId && a.DeletedAt == null)
            .ToListAsync();
    }

    
    public async Task<IEnumerable<Accessory>> GetByCompanyIdAsync(uint companyId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.CompanyId == companyId && a.DeletedAt == null)
            .ToListAsync();
    }

    
    public async Task<IEnumerable<Accessory>> SearchAccessoriesAsync(string searchTerm)
    {
        var term = $"%{searchTerm.Trim()}%";

        return await _dbSet
            .AsNoTracking()
            .Where(a => a.DeletedAt == null && (
                (a.Name != null && EF.Functions.Like(a.Name, term)) ||
                (a.ModelNumber != null && EF.Functions.Like(a.ModelNumber, term)) ||
                (a.OrderNumber != null && EF.Functions.Like(a.OrderNumber, term))
            ))
            .ToListAsync();
    }

    
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
            query = ApplyAvailableAccessoryFilter(query);
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
            var term = $"%{searchTerm.Trim()}%";
            query = query.Where(a =>
                (a.Name != null && EF.Functions.Like(a.Name, term)) ||
                (a.ModelNumber != null && EF.Functions.Like(a.ModelNumber, term)) ||
                (a.OrderNumber != null && EF.Functions.Like(a.OrderNumber, term))
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

    
    public async Task<bool> IsAvailableAsync(uint accessoryId, int requestedQuantity = 1)
    {
        var accessory = await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == accessoryId);

        if (accessory == null || accessory.DeletedAt != null)
            return false;

        return accessory.Requestable && accessory.Qty >= requestedQuantity;
    }

    
    public async Task<int> GetAvailableQuantityAsync(uint accessoryId)
    {
        var accessory = await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == accessoryId && a.DeletedAt == null);

        return accessory?.Qty ?? 0;
    }

    
    public async Task<IEnumerable<Accessory>> GetLowStockAccessoriesAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a =>
                a.DeletedAt == null &&
                a.Qty > 0 && // Не полностью закончились
                (
                    (a.MinAmt.HasValue && a.Qty <= a.MinAmt.Value) ||
                    (!a.MinAmt.HasValue && a.Qty < DefaultLowStockThreshold) // Default threshold
                ))
            .OrderBy(a => a.Qty) // Сортируем по количеству (меньше первыми)
            .ToListAsync();
    }

    
    public async Task<IEnumerable<Accessory>> GetOutOfStockAccessoriesAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.DeletedAt == null && a.Qty == 0)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    
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
            var search = $"%{searchTerm.Trim()}%";
            query = query.Where(x =>
                (x.accessory.Name != null && EF.Functions.Like(x.accessory.Name, search)) ||
                (x.accessory.ModelNumber != null && EF.Functions.Like(x.accessory.ModelNumber, search)));
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

    private static IQueryable<Accessory> ApplyAvailableAccessoryFilter(IQueryable<Accessory> query)
    {
        return query.Where(a => a.DeletedAt == null && a.Requestable && a.Qty > 0);
    }
}

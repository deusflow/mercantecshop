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
}
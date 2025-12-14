using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

/// <summary>
/// Order Repository Implementation
/// Реализация репозитория для работы с заказами (CheckoutRequests)
/// </summary>
public class OrderRepository : Repository<CheckoutRequest>, IOrderRepository
{
    public OrderRepository(SnipeItContext context) : base(context)
    {
    }

    /// <summary>
    /// Получить все заказы пользователя
    /// Сортируем по дате создания (новые первыми)
    /// </summary>
    public async Task<IEnumerable<CheckoutRequest>> GetUserOrdersAsync(int userId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(o => o.UserId == userId && o.DeletedAt == null)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Получить заказы пользователя с пагинацией
    /// </summary>
    public async Task<(IEnumerable<CheckoutRequest> Orders, int TotalCount)> GetUserOrdersPagedAsync(
        int userId, 
        int pageNumber, 
        int pageSize)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(o => o.UserId == userId && o.DeletedAt == null);
        
        var totalCount = await query.CountAsync();
        
        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (orders, totalCount);
    }

    /// <summary>
    /// Получить ожидающие заказы (Pending)
    /// Pending = CreatedAt есть, но FulfilledAt и CanceledAt = null
    /// </summary>
    public async Task<IEnumerable<CheckoutRequest>> GetPendingOrdersAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(o => 
                o.DeletedAt == null &&
                o.FulfilledAt == null &&
                o.CanceledAt == null)
            .OrderBy(o => o.CreatedAt) // Старые заказы первыми (очередь)
            .ToListAsync();
    }

    /// <summary>
    /// Получить выполненные заказы (Fulfilled)
    /// </summary>
    public async Task<IEnumerable<CheckoutRequest>> GetFulfilledOrdersAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(o => 
                o.DeletedAt == null &&
                o.FulfilledAt != null)
            .OrderByDescending(o => o.FulfilledAt)
            .ToListAsync();
    }

    /// <summary>
    /// Получить отмененные заказы (Canceled)
    /// </summary>
    public async Task<IEnumerable<CheckoutRequest>> GetCanceledOrdersAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(o => 
                o.DeletedAt == null &&
                o.CanceledAt != null)
            .OrderByDescending(o => o.CanceledAt)
            .ToListAsync();
    }

    /// <summary>
    /// Получить заказы по статусу с пагинацией
    /// </summary>
    public async Task<(IEnumerable<CheckoutRequest> Orders, int TotalCount)> GetOrdersByStatusPagedAsync(
        string status, 
        int pageNumber, 
        int pageSize)
    {
        // Строим запрос в зависимости от статуса
        IQueryable<CheckoutRequest> query = status.ToLower() switch
        {
            "pending" => _dbSet.AsNoTracking().Where(o =>
                o.DeletedAt == null &&
                o.FulfilledAt == null &&
                o.CanceledAt == null),
            
            "fulfilled" => _dbSet.AsNoTracking().Where(o =>
                o.DeletedAt == null &&
                o.FulfilledAt != null),
            
            "canceled" => _dbSet.AsNoTracking().Where(o =>
                o.DeletedAt == null &&
                o.CanceledAt != null),
            
            _ => _dbSet.AsNoTracking().Where(o => o.DeletedAt == null)
        };
        
        var totalCount = await query.CountAsync();
        
        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (orders, totalCount);
    }

    /// <summary>
    /// Получить все заказы с множественными фильтрами
    /// ОСНОВНОЙ МЕТОД ДЛЯ АДМИН-ПАНЕЛИ
    /// </summary>
    public async Task<(IEnumerable<CheckoutRequest> Orders, int TotalCount)> GetAllOrdersPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? status = null, 
        int? userId = null, 
        DateTime? fromDate = null, 
        DateTime? toDate = null)
    {
        var query = _dbSet.AsNoTracking().Where(o => o.DeletedAt == null);
        
        // Фильтр по статусу
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = status.ToLower() switch
            {
                "pending" => query.Where(o => o.FulfilledAt == null && o.CanceledAt == null),
                "fulfilled" => query.Where(o => o.FulfilledAt != null),
                "canceled" => query.Where(o => o.CanceledAt != null),
                _ => query
            };
        }
        
        // Фильтр по пользователю
        if (userId.HasValue)
        {
            query = query.Where(o => o.UserId == userId.Value);
        }
        
        // Фильтр по дате (от)
        if (fromDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= fromDate.Value);
        }
        
        // Фильтр по дате (до)
        if (toDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= toDate.Value);
        }
        
        var totalCount = await query.CountAsync();
        
        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (orders, totalCount);
    }

    /// <summary>
    /// Получить историю заказов для конкретного товара (Asset)
    /// </summary>
    public async Task<IEnumerable<CheckoutRequest>> GetOrdersForAssetAsync(int assetId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(o => 
                o.RequestableId == assetId &&
                o.RequestableType == "App\\Models\\Asset" &&
                o.DeletedAt == null)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Получить историю заказов для аксессуара
    /// </summary>
    public async Task<IEnumerable<CheckoutRequest>> GetOrdersForAccessoryAsync(int accessoryId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(o => 
                o.RequestableId == accessoryId &&
                o.RequestableType == "App\\Models\\Accessory" &&
                o.DeletedAt == null)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Проверить, есть ли у пользователя активный заказ на данный товар
    /// Предотвращает дубликаты заказов
    /// </summary>
    public async Task<bool> HasActivePendingOrderAsync(
        int userId, 
        int requestableId, 
        string requestableType)
    {
        return await _dbSet
            .AnyAsync(o =>
                o.UserId == userId &&
                o.RequestableId == requestableId &&
                o.RequestableType == requestableType &&
                o.FulfilledAt == null &&
                o.CanceledAt == null &&
                o.DeletedAt == null);
    }

    /// <summary>
    /// Подсчитать количество заказов пользователя
    /// </summary>
    public async Task<int> GetUserOrderCountAsync(int userId)
    {
        return await _dbSet
            .CountAsync(o => o.UserId == userId && o.DeletedAt == null);
    }

    /// <summary>
    /// Подсчитать количество ожидающих заказов
    /// Для badge в админ-панели
    /// </summary>
    public async Task<int> GetPendingOrderCountAsync()
    {
        return await _dbSet
            .CountAsync(o => 
                o.FulfilledAt == null &&
                o.CanceledAt == null &&
                o.DeletedAt == null);
    }

    /// <summary>
    /// Получить заказы за период
    /// Для статистики и отчетов
    /// </summary>
    public async Task<IEnumerable<CheckoutRequest>> GetOrdersByDateRangeAsync(
        DateTime fromDate, 
        DateTime toDate)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(o => 
                o.DeletedAt == null &&
                o.CreatedAt >= fromDate &&
                o.CreatedAt <= toDate)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Получить последние N заказов
    /// Для dashboard и recent activity
    /// </summary>
    public async Task<IEnumerable<CheckoutRequest>> GetRecentOrdersAsync(int count = 10)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(o => o.DeletedAt == null)
            .OrderByDescending(o => o.CreatedAt)
            .Take(count)
            .ToListAsync();
    }
}
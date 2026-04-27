using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

public class OrderRepository : Repository<CheckoutRequest>, IOrderRepository
{
    public OrderRepository(SnipeItContext context) : base(context)
    {
    }

    
    public async Task<IEnumerable<CheckoutRequest>> GetUserOrdersAsync(int userId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(o => o.UserId == userId && o.DeletedAt == null)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    
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

    
    public async Task<int> GetUserOrderCountAsync(int userId)
    {
        return await _dbSet
            .CountAsync(o => o.UserId == userId && o.DeletedAt == null);
    }

    
    public async Task<int> GetPendingOrderCountAsync()
    {
        return await _dbSet
            .CountAsync(o => 
                o.FulfilledAt == null &&
                o.CanceledAt == null &&
                o.DeletedAt == null);
    }

    
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
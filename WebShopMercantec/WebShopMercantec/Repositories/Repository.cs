using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    // DbContext is the Entity Framework "window" to the database
    protected readonly SnipeItContext _context;
    
    // DbSet is the database "table" for a specific entity
    protected readonly DbSet<T> _dbSet;

    
    public Repository(SnipeItContext context)
    {
        _context = context;
        _dbSet = context.Set<T>(); // Get the DbSet for type T
    }

    
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        // FindAsync is an optimized EF Core lookup by primary key
        return await _dbSet.FindAsync(id);
    }
    
    
    public virtual async Task<T?> GetByIdAsync(uint id)
    {
        return await _dbSet.FindAsync(id);
    }

    
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet
            .AsNoTracking() // No tracking for read-only queries
            .ToListAsync();
    }

    
    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(predicate) // LINQ filter
            .ToListAsync();
    }

    
    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate);
    }

    
    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize)
    {
        // Count total items
        var totalCount = await _dbSet.CountAsync();
        
        // Fetch the requested page
        var items = await _dbSet
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize) // Skip previous pages
            .Take(pageSize)                     // Take page size
            .ToListAsync();
        
        return (items, totalCount);
    }

    
    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        Expression<Func<T, bool>>? filter = null)
    {
        // Start from the base query
        IQueryable<T> query = _dbSet.AsNoTracking();
        
        // Apply filter if provided
        if (filter != null)
        {
            query = query.Where(filter);
        }
        
        // Count filtered items
        var totalCount = await query.CountAsync();
        
        // Fetch the page
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (items, totalCount);
    }

    
    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    
    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    
    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    
    public virtual void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    
    public virtual async Task<bool> DeleteByIdAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            return false;
        
        Delete(entity);
        return true;
    }

    
    public virtual async Task<bool> DeleteByIdAsync(uint id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            return false;
        
        Delete(entity);
        return true;
    }

    
    public virtual void DeleteRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    
    public virtual async Task<bool> ExistsAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        return entity != null;
    }

    
    public virtual async Task<bool> ExistsAsync(uint id)
    {
        var entity = await _dbSet.FindAsync(id);
        return entity != null;
    }

    
    public virtual async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.CountAsync(predicate);
    }

    
    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }
}
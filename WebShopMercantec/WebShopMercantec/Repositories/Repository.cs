using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    // DbContext - это "окно" в базу данных от Entity Framework
    protected readonly SnipeItContext _context;
    
    // DbSet - это "таблица" в базе данных для конкретной сущности
    protected readonly DbSet<T> _dbSet;

    
    public Repository(SnipeItContext context)
    {
        _context = context;
        _dbSet = context.Set<T>(); // Получаем DbSet для типа T
    }

    
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        // FindAsync - оптимизированный метод EF Core для поиска по первичному ключу
        return await _dbSet.FindAsync(id);
    }
    
    
    public virtual async Task<T?> GetByIdAsync(uint id)
    {
        return await _dbSet.FindAsync(id);
    }

    
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet
            .AsNoTracking() // Не отслеживаем изменения (read-only)
            .ToListAsync();
    }

    
    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(predicate) // LINQ фильтрация
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
        // Подсчитываем общее количество
        var totalCount = await _dbSet.CountAsync();
        
        // Получаем нужную "страницу" данных
        var items = await _dbSet
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize) // Пропускаем предыдущие страницы
            .Take(pageSize)                     // Берем нужное количество
            .ToListAsync();
        
        return (items, totalCount);
    }

    
    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        Expression<Func<T, bool>>? filter = null)
    {
        // Начинаем с базового запроса
        IQueryable<T> query = _dbSet.AsNoTracking();
        
        // Применяем фильтр, если он есть
        if (filter != null)
        {
            query = query.Where(filter);
        }
        
        // Считаем отфильтрованное количество
        var totalCount = await query.CountAsync();
        
        // Получаем страницу
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
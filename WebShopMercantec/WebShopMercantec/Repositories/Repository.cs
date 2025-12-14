using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories;

/// <summary>
/// Generic Repository Implementation - базовая реализация паттерна Repository
/// Инкапсулирует всю работу с Entity Framework Core и базой данных
/// Использует DbContext для выполнения операций CRUD
/// </summary>
/// <typeparam name="T">Тип сущности из базы данных</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    // DbContext - это "окно" в базу данных от Entity Framework
    protected readonly SnipeItContext _context;
    
    // DbSet - это "таблица" в базе данных для конкретной сущности
    protected readonly DbSet<T> _dbSet;

    /// <summary>
    /// Конструктор - принимает DbContext через Dependency Injection
    /// </summary>
    public Repository(SnipeItContext context)
    {
        _context = context;
        _dbSet = context.Set<T>(); // Получаем DbSet для типа T
    }

    /// <summary>
    /// Получить по ID (int версия)
    /// </summary>
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        // FindAsync - оптимизированный метод EF Core для поиска по первичному ключу
        return await _dbSet.FindAsync(id);
    }
    
    /// <summary>
    /// Получить по ID (uint версия для моделей SnipeIt)
    /// </summary>
    public virtual async Task<T?> GetByIdAsync(uint id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <summary>
    /// Получить все записи
    /// AsNoTracking - для read-only операций (быстрее, меньше памяти)
    /// </summary>
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet
            .AsNoTracking() // Не отслеживаем изменения (read-only)
            .ToListAsync();
    }

    /// <summary>
    /// Найти по условию
    /// Пример: repository.FindAsync(u => u.Email == "test@test.com")
    /// </summary>
    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(predicate) // LINQ фильтрация
            .ToListAsync();
    }

    /// <summary>
    /// Найти первую запись или вернуть null
    /// </summary>
    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(predicate);
    }

    /// <summary>
    /// Получить с пагинацией (простая версия)
    /// </summary>
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

    /// <summary>
    /// Получить с пагинацией и фильтром
    /// </summary>
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

    /// <summary>
    /// Добавить новую запись
    /// ВАЖНО: Изменения не сохраняются сразу в БД!
    /// Нужно вызвать SaveChangesAsync() через UnitOfWork
    /// </summary>
    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    /// <summary>
    /// Добавить несколько записей
    /// </summary>
    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    /// <summary>
    /// Обновить запись
    /// EF Core отслеживает изменения и обновит при SaveChanges
    /// </summary>
    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    /// <summary>
    /// Удалить запись
    /// </summary>
    public virtual void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    /// <summary>
    /// Удалить по ID (int)
    /// </summary>
    public virtual async Task<bool> DeleteByIdAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            return false;
        
        Delete(entity);
        return true;
    }

    /// <summary>
    /// Удалить по ID (uint)
    /// </summary>
    public virtual async Task<bool> DeleteByIdAsync(uint id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            return false;
        
        Delete(entity);
        return true;
    }

    /// <summary>
    /// Удалить несколько записей
    /// </summary>
    public virtual void DeleteRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    /// <summary>
    /// Проверить существование по ID (int)
    /// </summary>
    public virtual async Task<bool> ExistsAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        return entity != null;
    }

    /// <summary>
    /// Проверить существование по ID (uint)
    /// </summary>
    public virtual async Task<bool> ExistsAsync(uint id)
    {
        var entity = await _dbSet.FindAsync(id);
        return entity != null;
    }

    /// <summary>
    /// Подсчитать все записи
    /// </summary>
    public virtual async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    /// <summary>
    /// Подсчитать записи по условию
    /// </summary>
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.CountAsync(predicate);
    }

    /// <summary>
    /// Проверить наличие хотя бы одной записи по условию
    /// </summary>
    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }
}
using System.Linq.Expressions;

namespace WebShopMercantec.Repositories;

public interface IRepository<T> where T : class
{
    
    
    
    Task<T?> GetByIdAsync(int id);
    
    
    Task<T?> GetByIdAsync(uint id);
    
    
    
    Task<IEnumerable<T>> GetAllAsync();
    
    
    
    
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    
    
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    
    
    
    
    
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
    
    
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        Expression<Func<T, bool>>? filter = null);
    
    
    
    
    Task<T> AddAsync(T entity);
    
    
    Task AddRangeAsync(IEnumerable<T> entities);
    
    
    
    void Update(T entity);
    
    
    
    void Delete(T entity);
    
    
    Task<bool> DeleteByIdAsync(int id);
    
    
    Task<bool> DeleteByIdAsync(uint id);
    
    
    void DeleteRange(IEnumerable<T> entities);
    
    
    Task<bool> ExistsAsync(int id);
    
    
    Task<bool> ExistsAsync(uint id);
    
    
    Task<int> CountAsync();
    
    
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    
    
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
}
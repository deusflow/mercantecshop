using System.Linq.Expressions;

namespace WebShopMercantec.Repositories;

/// <summary>
/// Generic Repository Interface - базовый контракт для всех репозиториев
/// Определяет стандартные CRUD операции для работы с любой сущностью
/// </summary>
/// <typeparam name="T">Тип сущности (модель из БД)</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Получить сущность по ID
    /// </summary>
    /// <param name="id">ID сущности</param>
    /// <returns>Сущность или null, если не найдена</returns>
    Task<T?> GetByIdAsync(int id);
    
    /// <summary>
    /// Получить сущность по ID (uint версия для совместимости с моделями SnipeIt)
    /// </summary>
    Task<T?> GetByIdAsync(uint id);
    
    /// <summary>
    /// Получить все сущности
    /// ВНИМАНИЕ: Используйте с осторожностью на больших таблицах!
    /// </summary>
    /// <returns>Список всех сущностей</returns>
    Task<IEnumerable<T>> GetAllAsync();
    
    /// <summary>
    /// Найти сущности по условию (предикату)
    /// Пример: await repository.FindAsync(u => u.Email == "test@test.com");
    /// </summary>
    /// <param name="predicate">Условие для фильтрации (лямбда-выражение)</param>
    /// <returns>Список найденных сущностей</returns>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    
    /// <summary>
    /// Найти первую сущность по условию или вернуть null
    /// </summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    
    /// <summary>
    /// Получить сущности с пагинацией
    /// </summary>
    /// <param name="pageNumber">Номер страницы (начиная с 1)</param>
    /// <param name="pageSize">Количество элементов на странице</param>
    /// <returns>Кортеж: (список сущностей, общее количество)</returns>
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
    
    /// <summary>
    /// Получить сущности с пагинацией и фильтром
    /// </summary>
    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        Expression<Func<T, bool>>? filter = null);
    
    /// <summary>
    /// Добавить новую сущность
    /// ВАЖНО: Не сохраняет в БД! Нужно вызвать SaveChangesAsync через UnitOfWork
    /// </summary>
    /// <param name="entity">Сущность для добавления</param>
    /// <returns>Добавленная сущность</returns>
    Task<T> AddAsync(T entity);
    
    /// <summary>
    /// Добавить несколько сущностей
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities);
    
    /// <summary>
    /// Обновить сущность
    /// ВАЖНО: Не сохраняет в БД! Нужно вызвать SaveChangesAsync через UnitOfWork
    /// </summary>
    /// <param name="entity">Сущность для обновления</param>
    void Update(T entity);
    
    /// <summary>
    /// Удалить сущность
    /// ВАЖНО: Не сохраняет в БД! Нужно вызвать SaveChangesAsync через UnitOfWork
    /// </summary>
    /// <param name="entity">Сущность для удаления</param>
    void Delete(T entity);
    
    /// <summary>
    /// Удалить сущность по ID
    /// </summary>
    Task<bool> DeleteByIdAsync(int id);
    
    /// <summary>
    /// Удалить сущность по ID (uint версия)
    /// </summary>
    Task<bool> DeleteByIdAsync(uint id);
    
    /// <summary>
    /// Удалить несколько сущностей
    /// </summary>
    void DeleteRange(IEnumerable<T> entities);
    
    /// <summary>
    /// Проверить, существует ли сущность с данным ID
    /// </summary>
    Task<bool> ExistsAsync(int id);
    
    /// <summary>
    /// Проверить, существует ли сущность с данным ID (uint версия)
    /// </summary>
    Task<bool> ExistsAsync(uint id);
    
    /// <summary>
    /// Подсчитать количество сущностей
    /// </summary>
    Task<int> CountAsync();
    
    /// <summary>
    /// Подсчитать количество сущностей по условию
    /// </summary>
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    
    /// <summary>
    /// Проверить, есть ли хотя бы одна сущность по условию
    /// </summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
}
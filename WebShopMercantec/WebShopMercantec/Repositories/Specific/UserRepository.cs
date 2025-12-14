using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

/// <summary>
/// User Repository Implementation
/// Реализация репозитория для работы с пользователями
/// Наследуется от базового Repository и добавляет специфичные методы
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    /// <summary>
    /// Конструктор - передает контекст в базовый класс
    /// </summary>
    public UserRepository(SnipeItContext context) : base(context)
    {
    }

    /// <summary>
    /// Найти пользователя по email (case-insensitive)
    /// </summary>
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email!.ToLower() == email.ToLower());
    }

    /// <summary>
    /// Найти пользователя по username (case-insensitive)
    /// </summary>
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username!.ToLower() == username.ToLower());
    }

    /// <summary>
    /// Найти пользователя по email ИЛИ username
    /// Проверяет оба поля одновременно
    /// </summary>
    public async Task<User?> GetByEmailOrUsernameAsync(string emailOrUsername)
    {
        var searchTerm = emailOrUsername.ToLower();
        
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => 
                u.Email!.ToLower() == searchTerm || 
                u.Username!.ToLower() == searchTerm);
    }

    /// <summary>
    /// Проверить существование email
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet
            .AnyAsync(u => u.Email!.ToLower() == email.ToLower());
    }

    /// <summary>
    /// Проверить существование username
    /// </summary>
    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _dbSet
            .AnyAsync(u => u.Username!.ToLower() == username.ToLower());
    }

    /// <summary>
    /// Получить всех активированных пользователей
    /// Фильтруем по Activated = true и DeletedAt = null (не удаленные)
    /// </summary>
    public async Task<IEnumerable<User>> GetActivatedUsersAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(u => u.Activated && u.DeletedAt == null)
            .ToListAsync();
    }

    /// <summary>
    /// Получить пользователей по компании
    /// </summary>
    public async Task<IEnumerable<User>> GetByCompanyIdAsync(uint companyId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(u => u.CompanyId == companyId && u.DeletedAt == null)
            .ToListAsync();
    }

    /// <summary>
    /// Получить пользователей по локации
    /// </summary>
    public async Task<IEnumerable<User>> GetByLocationIdAsync(int locationId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(u => u.LocationId == locationId && u.DeletedAt == null)
            .ToListAsync();
    }

    /// <summary>
    /// Получить пользователей по департаменту
    /// </summary>
    public async Task<IEnumerable<User>> GetByDepartmentIdAsync(int departmentId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(u => u.DepartmentId == departmentId && u.DeletedAt == null)
            .ToListAsync();
    }

    /// <summary>
    /// Получить пользователей для отображения в списках
    /// ShowInList = true (по умолчанию true, но проверяем)
    /// </summary>
    public async Task<IEnumerable<User>> GetUsersForListAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(u => u.ShowInList != false && u.DeletedAt == null)
            .ToListAsync();
    }

    /// <summary>
    /// Поиск пользователей по строке
    /// Ищем в FirstName, LastName, Email, Username
    /// </summary>
    public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        
        return await _dbSet
            .AsNoTracking()
            .Where(u => u.DeletedAt == null && (
                (u.FirstName != null && u.FirstName.ToLower().Contains(term)) ||
                (u.LastName != null && u.LastName.ToLower().Contains(term)) ||
                (u.Email != null && u.Email.ToLower().Contains(term)) ||
                (u.Username != null && u.Username.ToLower().Contains(term))
            ))
            .ToListAsync();
    }

    /// <summary>
    /// Получить пользователей с пагинацией и множественными фильтрами
    /// Самый сложный и полезный метод для админ-панели
    /// </summary>
    public async Task<(IEnumerable<User> Users, int TotalCount)> GetUsersPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm = null, 
        bool? activated = null, 
        uint? companyId = null)
    {
        // Начинаем с базового запроса
        var query = _dbSet.AsNoTracking().Where(u => u.DeletedAt == null);
        
        // Применяем фильтры, если они заданы
        
        // Фильтр по поиску
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(u =>
                (u.FirstName != null && u.FirstName.ToLower().Contains(term)) ||
                (u.LastName != null && u.LastName.ToLower().Contains(term)) ||
                (u.Email != null && u.Email.ToLower().Contains(term)) ||
                (u.Username != null && u.Username.ToLower().Contains(term))
            );
        }
        
        // Фильтр по статусу активации
        if (activated.HasValue)
        {
            query = query.Where(u => u.Activated == activated.Value);
        }
        
        // Фильтр по компании
        if (companyId.HasValue)
        {
            query = query.Where(u => u.CompanyId == companyId.Value);
        }
        
        // Считаем общее количество (после применения фильтров)
        var totalCount = await query.CountAsync();
        
        // Получаем страницу с сортировкой по ID
        var users = await query
            .OrderBy(u => u.Id) // Сортировка для стабильной пагинации
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (users, totalCount);
    }
}
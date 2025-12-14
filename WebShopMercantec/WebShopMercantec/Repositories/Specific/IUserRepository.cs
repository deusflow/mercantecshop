using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

/// <summary>
/// User Repository Interface
/// Расширяет базовый IRepository дополнительными методами специфичными для User
/// 
/// ЗАЧЕМ НУЖЕН?
/// Базовый IRepository дает CRUD операции, но не знает о специфике User.
/// Здесь добавляются методы, которые имеют смысл только для пользователей:
/// - Поиск по email/username
/// - Проверка активации
/// - Получение пользователей по компании
/// - И т.д.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Найти пользователя по email
    /// Используется при логине и регистрации
    /// </summary>
    /// <param name="email">Email пользователя</param>
    /// <returns>Пользователь или null</returns>
    Task<User?> GetByEmailAsync(string email);
    
    /// <summary>
    /// Найти пользователя по username
    /// Используется при логине
    /// </summary>
    /// <param name="username">Username пользователя</param>
    /// <returns>Пользователь или null</returns>
    Task<User?> GetByUsernameAsync(string username);
    
    /// <summary>
    /// Найти пользователя по email ИЛИ username
    /// Полезно для универсального логина (можно войти и по email, и по username)
    /// </summary>
    /// <param name="emailOrUsername">Email или username</param>
    /// <returns>Пользователь или null</returns>
    Task<User?> GetByEmailOrUsernameAsync(string emailOrUsername);
    
    /// <summary>
    /// Проверить, существует ли email в системе
    /// Используется при регистрации для валидации
    /// </summary>
    Task<bool> EmailExistsAsync(string email);
    
    /// <summary>
    /// Проверить, существует ли username в системе
    /// </summary>
    Task<bool> UsernameExistsAsync(string username);
    
    /// <summary>
    /// Получить всех активированных пользователей
    /// Activated = true означает, что аккаунт подтвержден
    /// </summary>
    Task<IEnumerable<User>> GetActivatedUsersAsync();
    
    /// <summary>
    /// Получить пользователей по компании
    /// Полезно для фильтрации и отчетов
    /// </summary>
    Task<IEnumerable<User>> GetByCompanyIdAsync(uint companyId);
    
    /// <summary>
    /// Получить пользователей по локации
    /// </summary>
    Task<IEnumerable<User>> GetByLocationIdAsync(int locationId);
    
    /// <summary>
    /// Получить пользователей по департаменту
    /// </summary>
    Task<IEnumerable<User>> GetByDepartmentIdAsync(int departmentId);
    
    /// <summary>
    /// Получить пользователей которые должны отображаться в списках
    /// ShowInList = true
    /// </summary>
    Task<IEnumerable<User>> GetUsersForListAsync();
    
    /// <summary>
    /// Поиск пользователей по имени или email
    /// Используется для поисковой строки в админ-панели
    /// </summary>
    /// <param name="searchTerm">Строка поиска</param>
    /// <returns>Найденные пользователи</returns>
    Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);
    
    /// <summary>
    /// Получить пользователей с пагинацией и сортировкой
    /// </summary>
    Task<(IEnumerable<User> Users, int TotalCount)> GetUsersPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm = null,
        bool? activated = null,
        uint? companyId = null);
}
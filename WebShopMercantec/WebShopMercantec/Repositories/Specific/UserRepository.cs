using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

public class UserRepository : Repository<User>, IUserRepository
{
    
    public UserRepository(SnipeItContext context) : base(context)
    {
    }

    
    public async Task<User?> GetByEmailAsync(string email)
    {
        var normalized = email.Trim();

        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email != null && u.Email == normalized);
    }

    
    public async Task<User?> GetByUsernameAsync(string username)
    {
        var normalized = username.Trim();

        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username != null && u.Username == normalized);
    }

    
    public async Task<User?> GetByEmailOrUsernameAsync(string emailOrUsername)
    {
        var searchTerm = emailOrUsername.Trim();
        
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => 
                (u.Email != null && u.Email == searchTerm) ||
                (u.Username != null && u.Username == searchTerm));
    }

    
    public async Task<bool> EmailExistsAsync(string email)
    {
        var normalized = email.Trim();

        return await _dbSet
            .AnyAsync(u => u.Email != null && u.Email == normalized);
    }

    
    public async Task<bool> UsernameExistsAsync(string username)
    {
        var normalized = username.Trim();

        return await _dbSet
            .AnyAsync(u => u.Username != null && u.Username == normalized);
    }

    
    public async Task<IEnumerable<User>> GetActivatedUsersAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(u => u.Activated && u.DeletedAt == null)
            .ToListAsync();
    }

    
    public async Task<IEnumerable<User>> GetByCompanyIdAsync(uint companyId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(u => u.CompanyId == companyId && u.DeletedAt == null)
            .ToListAsync();
    }

    
    public async Task<IEnumerable<User>> GetByLocationIdAsync(int locationId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(u => u.LocationId == locationId && u.DeletedAt == null)
            .ToListAsync();
    }

    
    public async Task<IEnumerable<User>> GetByDepartmentIdAsync(int departmentId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(u => u.DepartmentId == departmentId && u.DeletedAt == null)
            .ToListAsync();
    }

    
    public async Task<IEnumerable<User>> GetUsersForListAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(u => u.ShowInList != false && u.DeletedAt == null)
            .ToListAsync();
    }

    
    public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
    {
        var term = $"%{searchTerm.Trim()}%";
        
        return await _dbSet
            .AsNoTracking()
            .Where(u => u.DeletedAt == null && (
                (u.FirstName != null && EF.Functions.Like(u.FirstName, term)) ||
                (u.LastName != null && EF.Functions.Like(u.LastName, term)) ||
                (u.Email != null && EF.Functions.Like(u.Email, term)) ||
                (u.Username != null && EF.Functions.Like(u.Username, term))
            ))
            .ToListAsync();
    }

    
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
            var term = $"%{searchTerm.Trim()}%";
            query = query.Where(u =>
                (u.FirstName != null && EF.Functions.Like(u.FirstName, term)) ||
                (u.LastName != null && EF.Functions.Like(u.LastName, term)) ||
                (u.Email != null && EF.Functions.Like(u.Email, term)) ||
                (u.Username != null && EF.Functions.Like(u.Username, term))
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
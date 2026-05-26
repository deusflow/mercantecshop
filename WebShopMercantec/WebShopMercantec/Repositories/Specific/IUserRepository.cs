using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

public interface IUserRepository : IRepository<User>
{
    
    
    
    Task<User?> GetByEmailAsync(string email);
    
    
    
    
    Task<User?> GetByUsernameAsync(string username);
    
    
    
    
    Task<User?> GetByEmailOrUsernameAsync(string emailOrUsername);
    
    
    Task<bool> EmailExistsAsync(string email);
    
    
    Task<bool> UsernameExistsAsync(string username);
    
    
    Task<IEnumerable<User>> GetActivatedUsersAsync();
    
    
    Task<IEnumerable<User>> GetByCompanyIdAsync(uint companyId);
    
    
    Task<IEnumerable<User>> GetByLocationIdAsync(int locationId);
    
    
    Task<IEnumerable<User>> GetByDepartmentIdAsync(int departmentId);
    
    
    Task<IEnumerable<User>> GetUsersForListAsync();
    
    
    
    
    Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);
    
    
    Task<(IEnumerable<User> Users, int TotalCount)> GetUsersPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? searchTerm = null,
        bool? activated = null,
        uint? companyId = null);
}
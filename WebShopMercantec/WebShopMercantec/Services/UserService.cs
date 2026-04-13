using WebShopMercantec.Exceptions;
using WebShopMercantec.Mapping;
using WebShopMercantec.Repositories;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;

    public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AdminStatsDto> GetAdminStatsAsync()
    {
        var totalUsers = await _unitOfWork.Context.Users.CountAsync(u => u.DeletedAt == null);
        var activeUsers = await _unitOfWork.Context.Users.CountAsync(u => u.DeletedAt == null && u.Activated);
        var totalCredits = await _unitOfWork.Context.WebShopUserCredits.SumAsync(c => c.AvailableCredits);
        var totalTxs = await _unitOfWork.Context.CreditTransactions.CountAsync();

        return new AdminStatsDto
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            TotalCredits = totalCredits,
            TotalTransactions = totalTxs
        };
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync((uint)id);
        if (user == null || user.DeletedAt != null)
            throw new NotFoundException("User", id);

        var credits = await _unitOfWork.Context.WebShopUserCredits
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == user.Id);

        return UserMapping.MapToDto(user, credits);
    }

    public async Task<UserDto?> GetCurrentUserAsync(int userId)
        => await GetByIdAsync(userId);

    public async Task<(IEnumerable<UserDto> Users, int TotalCount)> GetUsersPagedAsync(
        int page, int pageSize, string? search = null, string? filter = null)
    {
        var query = _unitOfWork.Context.Users.AsNoTracking().Where(u => u.DeletedAt == null);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u => 
                (u.FirstName != null && u.FirstName.Contains(search)) ||
                (u.LastName != null && u.LastName.Contains(search)) ||
                (u.Email != null && u.Email.Contains(search)) ||
                (u.Username != null && u.Username.Contains(search)));
        }

        if (filter == "active")
        {
            query = query.Where(u => u.Activated);
        }
        else if (filter == "hasTransactions")
        {
            query = query.Where(u => _unitOfWork.Context.CreditTransactions.Any(t => t.UserId == u.Id));
        }
        else if (filter == "hasCredits")
        {
            query = query.Where(u => _unitOfWork.Context.WebShopUserCredits.Any(c => c.UserId == u.Id && c.AvailableCredits > 0));
        }
        else if (filter == "noCredits")
        {
            query = query.Where(u => !_unitOfWork.Context.WebShopUserCredits.Any(c => c.UserId == u.Id && c.AvailableCredits > 0));
        }

        var total = await query.CountAsync();
        var users = await query.OrderBy(u => u.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var userIds = users.Select(u => (uint)u.Id).ToList();
        
        // Fetch credits
        var creditsMap = await _unitOfWork.Context.WebShopUserCredits
            .AsNoTracking()
            .Where(c => userIds.Contains(c.UserId))
            .ToDictionaryAsync(c => c.UserId, c => c);
            
        // Fetch checked out items count
        var checkedOutCounts = await _unitOfWork.Context.Assets
            .AsNoTracking()
            .Where(a => a.AssignedTo != null && userIds.Contains((uint)a.AssignedTo))
            .GroupBy(a => a.AssignedTo)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => (uint)g.UserId!, g => g.Count);

        var result = users.Select(u => 
        {
            creditsMap.TryGetValue((uint)u.Id, out var credits);
            checkedOutCounts.TryGetValue((uint)u.Id, out var itemsCount);
            
            var dto = UserMapping.MapToDto(u, credits);
            dto.CheckedOutItemsCount = itemsCount;
            dto.IsDebtor = itemsCount > 0 || (credits != null && credits.AvailableCredits < 0);
            return dto;
        });

        return (result, total);
    }

    public async Task<UserDto> UpdateProfileAsync(int userId, UserDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync((uint)userId);
        if (user == null || user.DeletedAt != null)
            throw new NotFoundException("User", userId);

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.Phone = dto.Phone;
        user.Jobtitle = dto.Jobtitle;
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User {UserId} updated profile", userId);
        return UserMapping.MapToDto(user);
    }
}

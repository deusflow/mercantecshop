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
        int page, int pageSize, string? search = null)
    {
        var (users, total) = await _unitOfWork.Users.GetUsersPagedAsync(page, pageSize, search);
        return (users.Select(u => UserMapping.MapToDto(u)), total);
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


using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Services;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(int id);
    Task<UserDto?> GetCurrentUserAsync(int userId);
    Task<(IEnumerable<UserDto> Users, int TotalCount)> GetUsersPagedAsync(int page, int pageSize, string? search = null);
    Task<UserDto> UpdateProfileAsync(int userId, UserDto dto);
}


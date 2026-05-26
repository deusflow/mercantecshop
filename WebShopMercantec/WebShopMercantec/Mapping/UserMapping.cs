using WebShopMercantec.Models;
using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Mapping;

public static class UserMapping
{
    public static UserDto MapToDto(User user, WebShopUserCredits? credits = null)
    {
        return new UserDto
        {
            Id = (int)user.Id,
            Username = user.Username ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            Avatar = user.Avatar,
            Role = ResolveRole(user),
            Permissions = user.Permissions,
            AvailableCredits = credits?.AvailableCredits ?? 0m,
            TotalCreditsSpent = credits?.TotalSpent ?? 0m,
            Phone = user.Phone,
            Jobtitle = user.Jobtitle,
            LocationId = user.LocationId,
            DepartmentId = user.DepartmentId,
            CompanyId = (int?)user.CompanyId,
            Address = user.Address,
            City = user.City,
            State = user.State,
            Country = user.Country,
            Zip = user.Zip,
            ManagerId = user.ManagerId,
            EmployeeNum = user.EmployeeNum,
            IsActive = user.Activated && user.DeletedAt == null,
            Activated = user.Activated,
            ShowInList = user.ShowInList ?? true,
            Vip = user.Vip ?? false,
            Remote = user.Remote ?? false,
            StartDate = user.StartDate.HasValue
                ? DateTime.Parse(user.StartDate.Value.ToString("yyyy-MM-dd"))
                : null,
            EndDate = user.EndDate.HasValue
                ? DateTime.Parse(user.EndDate.Value.ToString("yyyy-MM-dd"))
                : null,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            LastLogin = user.LastLogin,
            Notes = user.Notes,
            Website = user.Website
        };
    }

    private static string ResolveRole(User user)
    {
        if (user.Permissions == null) return "User";
        if (user.Permissions.Contains("\"superadmin\":\"1\"") ||
            user.Permissions.Contains("\"superadmin\": \"1\"")) return "Admin";
        if (user.Permissions.Contains("\"admin\":\"1\"") ||
            user.Permissions.Contains("\"admin\": \"1\"")) return "Admin";
        return "User";
    }
}


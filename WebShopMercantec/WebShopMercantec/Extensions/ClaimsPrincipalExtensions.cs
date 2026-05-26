using System.Security.Claims;

namespace WebShopMercantec.Extensions;

public static class ClaimsPrincipalExtensions
{
    
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue("userId")
                 ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
                 ?? user.FindFirstValue("sub");

        if (!int.TryParse(value, out var id) || id <= 0)
            throw new UnauthorizedException("Invalid user token");

        return id;
    }

    
    public static string GetRole(this ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Role) ?? "User";

    
    public static bool IsAdmin(this ClaimsPrincipal user)
        => user.IsInRole("Admin");
}


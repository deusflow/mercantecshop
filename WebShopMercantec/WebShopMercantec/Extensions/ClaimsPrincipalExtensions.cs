using System.Security.Claims;

namespace WebShopMercantec.Extensions;

/// <summary>
/// Extensions for reading JWT claims from HttpContext.User
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>Returns the authenticated user's ID from JWT claims</summary>
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue("userId")
                 ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
                 ?? user.FindFirstValue("sub");

        return int.TryParse(value, out var id) ? id : 0;
    }

    /// <summary>Returns the authenticated user's role ("Admin" or "User")</summary>
    public static string GetRole(this ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Role) ?? "User";

    /// <summary>Returns true if the current user is an Admin</summary>
    public static bool IsAdmin(this ClaimsPrincipal user)
        => user.IsInRole("Admin");
}


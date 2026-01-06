namespace WebShopMercantec.Services
{
    public interface IJwtService
    {
        string GenerateToken(UserDto user, string? roleName = null);
    }
}

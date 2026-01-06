namespace WebShopMercantec.Services
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByLogin(string login);
    }
}

using Microsoft.EntityFrameworkCore;

namespace WebShopMercantec.Services
{
    public class UserService : IUserService
    {
        private readonly SnipeItContext _context;

        public UserService(SnipeItContext context)
        {
            _context = context;
        }

        public async Task<UserDto?> GetUserByLogin(string login) {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == login.ToLower() || u.Email == login.ToLower());
            if (user != null) {
                UserDto result = new UserDto
                {
                    Id = (int)user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = "User"
                };

                return result;
            }
            return null;
        }
     }
}

using Microsoft.AspNetCore.Identity;
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<Users> _userManager;

        public UserRepository(UserManager<Users> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Users> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task CreateUserAsync(Users user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        public async Task<bool> CheckPasswordAsync(Users user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

    }
}

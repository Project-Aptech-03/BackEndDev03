using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;

namespace ProjectDemoWebApi.Repositories
{
    public class UserRepository : IUserRepository
    {
    readonly UserManager<Users> _userManager;
    
    public UserRepository(UserManager<Users> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<Users>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            return await _userManager.Users.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<Users?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }

        public async Task<Users?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == username, cancellationToken);
        }
        public async Task<Users?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
        
    }
}

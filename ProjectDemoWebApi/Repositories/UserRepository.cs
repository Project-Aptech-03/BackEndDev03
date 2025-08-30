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

        public async Task<(List<Users> Users, int TotalCount)> GetAllUsersdAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _userManager.Users.AsNoTracking();

            var totalCount = await query.CountAsync(cancellationToken); 

            var users = await query
                .OrderBy(u => u.UserName)           
                .Skip((pageIndex - 1) * pageSize)   
                .Take(pageSize)                 
                .ToListAsync(cancellationToken);

            return (users, totalCount);
        }

        public async Task<IdentityResult> CreateUserAsync(Users user, string password, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser != null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "DuplicateEmail",
                    Description = "Email đã được sử dụng."
                });
            }
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                user.UserName = user.Email;
            }
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

            return result;
        }


        public async Task<Users?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _userManager.FindByIdAsync(userId);
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

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.DTOs.User;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;
using System.Text;

namespace ProjectDemoWebApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        readonly UserManager<Users> _userManager;
        readonly IConfiguration _config;
        readonly IEmailService _emailService;
    
        public UserRepository(UserManager<Users> userManager, IConfiguration config,IEmailService emailService)
        {
            _emailService = emailService;
            _userManager = userManager;
            _config = config;

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

        //delete user by id
        public async Task<IdentityResult> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "Người dùng không tồn tại."
                });
            }
            var role = await _userManager.GetRolesAsync(user);
            if (role.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "CannotDeleteAdmin",
                    Description = "Không thể xóa người dùng với vai trò Admin."
                });
            }
            var result = await _userManager.DeleteAsync(user);
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

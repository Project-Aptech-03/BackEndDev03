using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Data;
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
        private readonly ApplicationDbContext _context;

        public UserRepository(UserManager<Users> userManager, IConfiguration config,IEmailService emailService, ApplicationDbContext context)
        {
            _emailService = emailService;
            _userManager = userManager;
            _config = config;
            _context = context;


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
                    Description = "Email is already in use. "
                });
            }

            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                user.UserName = user.Email;
            }

            return await _userManager.CreateAsync(user, password);
        }


        public async Task<IdentityResult> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "User does not exist."
                });
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any(r => r.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "CannotDeleteAdmin",
                    Description = "Cannot delete a user with the Admin role."
                });
            }

            var hasOrders = await _context.Orders.AnyAsync(o => o.CustomerId == user.Id, cancellationToken);
            var hasAddresses = await _context.CustomerAddresses.AnyAsync(a => a.UserId == user.Id, cancellationToken);

            if (hasOrders || hasAddresses)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "HasRelatedData",
                    Description = "Cannot delete the user because related data (orders or addresses) still exist."
                });
            }

            // If no related data → allow deletion
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

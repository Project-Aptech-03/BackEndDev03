using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ProjectDemoWebApi.DTOs.User;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Services
{
    public class UserService : IUserService
    {
        readonly IUserRepository _userRepository;
        readonly IMapper _mapper;
        readonly UserManager<Users> _userManager;
        readonly RoleManager<IdentityRole> _roleManager;

        public UserService(IUserRepository userRepository, UserManager<Users> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<Users>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetAllUsersAsync(cancellationToken);
        }

        public async Task<Users?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetUserByIdAsync(userId, cancellationToken);
        }
        public async Task<Users?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetUserByUsernameAsync(username, cancellationToken);
        }
        public async Task<Users?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetUserByEmailAsync(email, cancellationToken);
        }
        //public async Task<IdentityResult> CreateUserAsync(, CancellationToken cancellationToken = default)
        //{

        //}
        public async Task<IdentityResult> UpdateUserAsync(string id, UpdateUserDto userDto, CancellationToken cancellationToken)
        {
            // Validate dependencies
            if (_userManager == null || _mapper == null)
                return IdentityResult.Failed(new IdentityError { Description = "Internal service error" });

            var account = await _userManager.FindByIdAsync(id);
            if (account == null)
                return IdentityResult.Failed(new IdentityError { Description = "Tài khoản không tồn tại" });

            _mapper.Map(userDto, account);

            var updateResult = await _userManager.UpdateAsync(account);
            if (updateResult == null) // Check for null result
                return IdentityResult.Failed(new IdentityError { Description = "Update operation failed unexpectedly" });

            if (!updateResult.Succeeded)
                return updateResult;

            if (!string.IsNullOrEmpty(userDto.Role))
            {
                // Check if the role exists
                var roleExists = await _roleManager.RoleExistsAsync(userDto.Role);
                if (!roleExists)
                    return IdentityResult.Failed(new IdentityError { Description = "Vai trò không tồn tại" });

                var currentRoles = await _userManager.GetRolesAsync(account);
                if (currentRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(account, currentRoles);
                }
                await _userManager.AddToRoleAsync(account, userDto.Role);
            }

            return IdentityResult.Success;
        }
        //public async Task<IdentityResult> DeleteUserAsync(Users user, CancellationToken cancellationToken = default)
        //{
        //    return await _userRepository.DeleteUserAsync(user, cancellationToken);
        //}
    }
}

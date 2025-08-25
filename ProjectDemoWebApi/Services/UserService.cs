using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ProjectDemoWebApi.DTOs.Shared;
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

        public async Task<ApiResponse<List<UsersResponseDto>>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync(cancellationToken);
                var userDtos = _mapper.Map<List<UsersResponseDto>>(users);

                return ApiResponse<List<UsersResponseDto>>.Ok(
                    data: userDtos,
                    message: "Users retrieved successfully",
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<List<UsersResponseDto>>.Fail(
                    "An error occurred while retrieving users.",
                    null,
                    500
                );
            }
        }

        public async Task<ApiResponse<UsersResponseDto?>> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ApiResponse<UsersResponseDto?>.Fail(
                        "User ID cannot be empty.",
                        null,
                        400
                    );
                }

                var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

                if (user == null)
                {
                    return ApiResponse<UsersResponseDto?>.Fail(
                        "User not found.",
                        null,
                        404
                    );
                }

                var userDto = _mapper.Map<UsersResponseDto>(user);

                return ApiResponse<UsersResponseDto?>.Ok(
                    userDto,
                    "User retrieved successfully.",
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<UsersResponseDto?>.Fail(
                    "An error occurred while retrieving the user.",
                    null,
                    500
                );
            }
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

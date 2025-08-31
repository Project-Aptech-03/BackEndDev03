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
        readonly RoleManager<Roles> _roleManager;

        public UserService(IUserRepository userRepository, UserManager<Users> userManager, IMapper mapper, RoleManager<Roles> roleManager)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> CreateUserAsync(Users user, string password, CancellationToken cancellationToken = default)
        {
            return await _userRepository.CreateUserAsync(user, password, cancellationToken);
        }

        public async Task<ApiResponse<PagedResponseDto<UsersResponseDto>>> GetAllUsersAsync(
         int pageIndex = 1,
         int pageSize = 10,
         CancellationToken cancellationToken = default)
        {
            try
            {
                
                var (users, totalCount) = await _userRepository.GetAllUsersdAsync(pageIndex, pageSize, cancellationToken);
                var userDtos = _mapper.Map<List<UsersResponseDto>>(users);

                for (int i = 0; i < users.Count; i++)
                {
                    var roles = await _userManager.GetRolesAsync(users[i]);
                    userDtos[i].Role = roles.FirstOrDefault();
                }
                var response = new PagedResponseDto<UsersResponseDto>
                {
                    Items = userDtos,
                    TotalCount = totalCount,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };

                return ApiResponse<PagedResponseDto<UsersResponseDto>>.Ok(
                    data: response,
                    message: "Users retrieved successfully",
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<PagedResponseDto<UsersResponseDto>>.Fail(
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
                var roles = await _userManager.GetRolesAsync(user);
                userDto.Role = roles.FirstOrDefault(); 

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
      
        public async Task<IdentityResult> UpdateUserAsync(string id, UpdateUserDto userDto, CancellationToken cancellationToken)
        {
            if (_userManager == null || _mapper == null)
                return IdentityResult.Failed(new IdentityError { Description = "Internal service error" });

            var account = await _userManager.FindByIdAsync(id);
            if (account == null)
                return IdentityResult.Failed(new IdentityError { Description = "Tài khoản không tồn tại" });

            _mapper.Map(userDto, account);

            var updateResult = await _userManager.UpdateAsync(account);
            if (updateResult == null)
                return IdentityResult.Failed(new IdentityError { Description = "Update operation failed unexpectedly" });

            if (!updateResult.Succeeded)
                return updateResult;

            if (!string.IsNullOrEmpty(userDto.Role))
            {
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

        public async Task<IdentityResult> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "Người dùng không tồn tại" });

            return await _userRepository.DeleteUserAsync(userId, cancellationToken);
        }

        public async Task<IdentityResult> ResetPasswordAsync(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            return await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
        }




        // profile
        public async Task<ApiResponse<ProfileResponseDto>> GetProfile(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                return ApiResponse<ProfileResponseDto?>.Fail(
                "User ID cannot be empty.",
                    null,
                        400
                    );
                }
                var user = await _userRepository.GetByIdAsync(id, cancellationToken);
                if (user == null)
                {
                    return ApiResponse<ProfileResponseDto?>.Fail(
                        "User not found.",
                        null,
                        404
                    );
                }
                var userDto = _mapper.Map<ProfileResponseDto>(user);
                var roles = await _userManager.GetRolesAsync(user);
                userDto.Role = roles.FirstOrDefault();

                return ApiResponse<ProfileResponseDto?>.Ok(
                    userDto,
                    "User retrieved successfully.",
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<ProfileResponseDto?>.Fail(
                    "An error occurred while retrieving the user.",
                    null,
                    500
                );
            }
        }
        public async Task<IdentityResult> UpdateProfileAsync(string id, ProfileUpdateDto userDto, CancellationToken cancellationToken)
        {
            if (_userManager == null || _mapper == null)
                return IdentityResult.Failed(new IdentityError { Description = "Internal service error" });

            var account = await _userManager.FindByIdAsync(id);
            if (account == null)
                return IdentityResult.Failed(new IdentityError { Description = "Tài khoản không tồn tại" });

            _mapper.Map(userDto, account);

            var updateResult = await _userManager.UpdateAsync(account);
            if (updateResult == null)
                return IdentityResult.Failed(new IdentityError { Description = "Update operation failed unexpectedly" });

            if (!updateResult.Succeeded)
                return updateResult;

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });

            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }






    }
}

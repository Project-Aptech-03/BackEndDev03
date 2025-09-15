using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.DTOs.User;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;
using System.Security.Claims;

namespace ProjectDemoWebApi.Services
{
    public class UserService : IUserService
    {
        readonly IUserRepository _userRepository;
        readonly IMapper _mapper;
        readonly UserManager<Users> _userManager;
        readonly RoleManager<Roles> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        readonly IEmailService _emailService;

        public UserService(IUserRepository userRepository, UserManager<Users> userManager, IMapper mapper, RoleManager<Roles> roleManager, IHttpContextAccessor httpContextAccessor, IEmailService emailService )
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
        }

        public async Task<IdentityResult> CreateUserAsync(Users user, string password, CancellationToken cancellationToken = default)
        {
            var result = await _userRepository.CreateUserAsync(user, password, cancellationToken);

            if (result.Succeeded)
            {
                var subject = "📚 Chào mừng bạn đến với Nhà Sách Project03!";

                var body = $@"
        <div style='font-family: Arial, sans-serif; background:#fafafa; padding:20px;'>
            <div style='max-width:600px; margin:0 auto; background:#ffffff; border-radius:10px; box-shadow:0 2px 8px rgba(0,0,0,0.05); padding:30px;'>
                
                <h1 style='color:#2c3e50; text-align:center;'>✨ Chào mừng bạn, {user.FirstName} {user.LastName}! ✨</h1>
                
                <p style='font-size:16px; color:#444;'>
                    Cảm ơn bạn đã đăng ký tài khoản tại <b>Nhà Sách Project03</b>. 
                    Chúng tôi rất vui khi được đồng hành cùng bạn trên hành trình khám phá tri thức và niềm vui đọc sách.
                </p>

                <div style='background:#f0f8ff; padding:15px; border-left:5px solid #4CAF50; border-radius:6px; margin:20px 0;'>
                    <p style='margin:5px 0; font-size:15px;'><b>Email đăng nhập:</b> {user.Email}</p>
                    <p style='margin:5px 0; font-size:15px;'><b>Mật khẩu:</b> {password}</p>
                </div>

                <p style='font-size:15px; color:#555;'>
                    Hãy đăng nhập để bắt đầu hành trình cùng những cuốn sách hay dành cho bạn và bé! 📖👶
                </p>

                <div style='text-align:center; margin:30px 0;'>
                    <a href='http://localhost:3000/login' 
                       style='background:#4CAF50; color:#fff; text-decoration:none; padding:12px 25px; border-radius:6px; font-size:16px; display:inline-block;'>
                        Đăng nhập ngay
                    </a>
                </div>

                <hr style='margin:30px 0; border:none; border-top:1px solid #eee;'/>
                
                <p style='font-size:13px; color:#888; text-align:center;'>
                    Đây là email tự động, vui lòng không trả lời lại.<br/>
                    © {DateTime.Now.Year} Nhà Sách Project03. Tất cả các quyền được bảo lưu.
                </p>
            </div>
        </div>";

                await _emailService.SendEmailAsync(user.Email, subject, body);
            }

            return result;
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
                var currentUserId = _httpContextAccessor.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);

                if (currentUserId == id)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "Bạn không thể thay đổi vai trò của chính mình." });
                }

                var roleExists = await _roleManager.RoleExistsAsync(userDto.Role);
                if (!roleExists)
                    return IdentityResult.Failed(new IdentityError { Description = "Vai trò không tồn tại" });

                var isCurrentlyAdmin = await _userManager.IsInRoleAsync(account, "Admin");
                if (isCurrentlyAdmin && userDto.Role != "User")
                {
                    var adminCount = (await _userManager.GetUsersInRoleAsync("Admin")).Count;
                    if (adminCount <= 1)
                    {
                        return IdentityResult.Failed(new IdentityError { Description = "Không thể thay đổi vai trò vì hệ thống cần ít nhất một Admin." });
                    }
                }

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

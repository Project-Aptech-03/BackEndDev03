using ProjectDemoWebApi.DTOs.Auth;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.DTOs.User;
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IAuthService
    {
        Task<OtpResultDto>SendRegisterOtpAsync(RegisterRequest request);
        Task<RegisterResultDto>VerifyRegisterAsync(VerifyRegisterRequest request);

        Task<LoginResultDto> LoginAsync(LoginRequest request);
        Task<OtpResultDto> ResendRegisterOtpAsync(string email);

        Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto dto);
        Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordDto dto);
    }
}

using ProjectDemoWebApi.DTOs.Auth;
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IAuthService
    {
        Task<OtpResultDto>SendRegisterOtpAsync(RegisterRequest request);
        Task<RegisterResultDto>VerifyRegisterAsync(VerifyRegisterRequest request);

        Task<LoginResultDto> LoginAsync(LoginRequest request);



    }
}

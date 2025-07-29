using ProjectDemoWebApi.DTOs.Request;
using ProjectDemoWebApi.DTOs.Response;
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IUserService
    {
        Task<OtpResultDto>SendRegisterOtpAsync(RegisterRequest request);
        Task<RegisterResultDto>VerifyRegisterAsync(VerifyRegisterRequest request);

        Task<LoginResult> LoginAsync(LoginRequest request);



    }
}

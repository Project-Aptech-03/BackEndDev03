using ProjectDemoWebApi.DTOs.Request;
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Services
{
    public interface IUserService
    {
        Task SendRegisterOtpAsync(RegisterRequest request);
        Task VerifyRegisterAsync(VerifyRegisterRequest request);
        //Task RegisterAsync(RegisterRequest request);
        //Task<bool> LoginAsync(LoginRequest request);
        //Task<Users?> GetByEmailAsync(string email);
    }
}

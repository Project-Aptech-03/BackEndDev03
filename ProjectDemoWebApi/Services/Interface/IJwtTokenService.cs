using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IJwtTokenService
    {
        Task<string> GenerateTokenAsync(Users user);
    }

}

using ProjectDemoWebApi.DTOs.Auth;
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IJwtTokenService
    {
        Task<TokenResultDto> GenerateTokenAsync(Users user);      
        Task<string> GenerateRefreshTokenAsync(Users user);       
        Task<TokenResultDto> RefreshTokenAsync(string refreshToken);
    }
}

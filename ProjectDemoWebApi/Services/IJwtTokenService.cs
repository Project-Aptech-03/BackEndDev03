using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(Users user);
    }
}

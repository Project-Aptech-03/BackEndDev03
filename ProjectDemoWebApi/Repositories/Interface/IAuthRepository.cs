using Microsoft.AspNetCore.Identity;
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IAuthRepository
    {
        Task<Users> GetByEmailAsync(string email);
        Task CreateUserAsync(Users user, string password);

        Task<bool> CheckPasswordAsync(Users user, string password);
    }

}

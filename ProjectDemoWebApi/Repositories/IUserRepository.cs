using Microsoft.AspNetCore.Identity;
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories
{
    public interface IUserRepository
    {
        Task<Users> GetByEmailAsync(string email);
        Task CreateUserAsync(Users user, string password);

        Task<bool> CheckPasswordAsync(Users user, string password);
    }

}

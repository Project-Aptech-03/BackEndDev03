using Microsoft.AspNetCore.Identity;
using ProjectDemoWebApi.DTOs.User;
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IUserService
    {

        Task<List<Users>> GetAllUsersAsync(CancellationToken cancellationToken = default);
        Task<Users?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<Users?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
        
        Task<Users?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
        //Task<IdentityResult> CreateUserAsync(Users user, string password, CancellationToken cancellationToken = default);
        Task<IdentityResult> UpdateUserAsync(string id, UpdateUserDto userDto, CancellationToken cancellationToken);
        //Task<IdentityResult> DeleteUserAsync(Users user, CancellationToken cancellationToken = default);

    }
}

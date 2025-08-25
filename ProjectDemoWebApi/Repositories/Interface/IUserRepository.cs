using Microsoft.AspNetCore.Identity;
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IUserRepository
    {

        Task<List<Users>> GetAllUsersAsync(CancellationToken cancellationToken = default);
        Task<Users?> GetByIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<Users?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<Users?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);

    }
}

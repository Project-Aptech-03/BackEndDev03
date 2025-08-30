using Microsoft.AspNetCore.Identity;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.DTOs.User;
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IUserService
    {
        Task<IdentityResult> CreateUserAsync(Users user, string password, CancellationToken cancellationToken = default);
        Task<ApiResponse<PagedResponseDto<UsersResponseDto>>> GetAllUsersAsync(
         int pageIndex = 1,
         int pageSize = 10,
         CancellationToken cancellationToken = default);
        Task<ApiResponse<UsersResponseDto?>> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<Users?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
        
        Task<Users?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
        //Task<IdentityResult> CreateUserAsync(Users user, string password, CancellationToken cancellationToken = default);
        Task<IdentityResult> UpdateUserAsync(string id, UpdateUserDto userDto, CancellationToken cancellationToken);
        //Task<IdentityResult> DeleteUserAsync(Users user, CancellationToken cancellationToken = default);

    }
}

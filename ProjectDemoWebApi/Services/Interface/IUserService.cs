using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Services.Interface
{
    public interface IUserService
    {

        Task<List<Users>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    }
}

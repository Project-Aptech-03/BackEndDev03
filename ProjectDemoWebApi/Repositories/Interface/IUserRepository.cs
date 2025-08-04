using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Repositories.Interface
{
    public interface IUserRepository
    {

        Task<List<Users>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    }
}

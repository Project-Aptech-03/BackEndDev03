using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Services
{
    public class UserService : IUserService
    {
        readonly IUserRepository _userRepository;
        
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<List<Users>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetAllUsersAsync(cancellationToken);
        }

    }
}

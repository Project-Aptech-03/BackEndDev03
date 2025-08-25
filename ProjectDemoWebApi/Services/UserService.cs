using AutoMapper;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.DTOs.User;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        
        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<UsersResponseDto>>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync(cancellationToken);
                var userDtos = _mapper.Map<List<UsersResponseDto>>(users);

                return ApiResponse<List<UsersResponseDto>>.Ok(
                    data: userDtos,
                    message: "Users retrieved successfully",
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<List<UsersResponseDto>>.Fail(
                    "An error occurred while retrieving users.", 
                    null, 
                    500
                );
            }
        }

        public async Task<ApiResponse<UsersResponseDto?>> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    return ApiResponse<UsersResponseDto?>.Fail(
                        "User ID cannot be empty.", 
                        null, 
                        400
                    );
                }

                var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
                
                if (user == null)
                {
                    return ApiResponse<UsersResponseDto?>.Fail(
                        "User not found.", 
                        null, 
                        404
                    );
                }

                var userDto = _mapper.Map<UsersResponseDto>(user);
                
                return ApiResponse<UsersResponseDto?>.Ok(
                    userDto, 
                    "User retrieved successfully.", 
                    200
                );
            }
            catch (Exception ex)
            {
                return ApiResponse<UsersResponseDto?>.Fail(
                    "An error occurred while retrieving the user.", 
                    null, 
                    500
                );
            }
        }
    }
}

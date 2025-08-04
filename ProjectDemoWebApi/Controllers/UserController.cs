using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectDemoWebApi.DTOs.Response;
using ProjectDemoWebApi.DTOs.User;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;


        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllUsersAsync(cancellationToken);
            var userDtos = _mapper.Map<List<UsersResponseDto>>(users);

            return Ok(ApiResponse<List<UsersResponseDto>>.Ok(
                data: userDtos,
                message: "Lấy danh sách người dùng thành công",
                statusCode: 200
            ));
        }
    }
}

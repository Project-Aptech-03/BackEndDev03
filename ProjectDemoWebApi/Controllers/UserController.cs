    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using ProjectDemoWebApi.DTOs.Shared;
    using ProjectDemoWebApi.DTOs.User;
    using ProjectDemoWebApi.Services.Interface;
    using System.Security.Claims;

namespace ProjectDemoWebApi.Controllers
{
    [Authorize]
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

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User not authenticated");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();
            return StatusCode(result.StatusCode, result);
        }

        //[HttpGet("{id}")]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> GetUser(string id)
        //{
        //    var result = await _userService.GetUserByIdAsync(id);
        //    return StatusCode(result.StatusCode, result);
        //}

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(string userId, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByIdAsync(userId, cancellationToken);
            if (user == null || user.Data == null)
            {
                return NotFound(ApiResponse<string>.Fail(
                    message: "Người dùng không tồn tại",
                    statusCode: 404
                ));
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.UpdateUserAsync(userId, dto, cancellationToken);

            if (!result.Succeeded)
            {
                return BadRequest(ApiResponse<UsersResponseDto>.Fail(
                    "Cập nhật thất bại",
                    result.Errors.Select(e => e.Description).ToList()
                ));
            }

            var updatedUserResponse = await _userService.GetUserByIdAsync(userId, cancellationToken);
            if (updatedUserResponse == null)
                return NotFound(ApiResponse<UsersResponseDto>.Fail("Không tìm thấy user sau khi cập nhật"));

            var updatedUser = updatedUserResponse.Data;
            var responseDto = _mapper.Map<UsersResponseDto>(updatedUser);

            return Ok(ApiResponse<UsersResponseDto>.Ok(responseDto, "Cập nhật thành công"));
        }
    
            //[HttpPost]
            //public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto createUserRequestDto, CancellationToken cancellationToken)
            //{
            //    var user = _mapper.Map<ProjectDemoWebApi.Models.Users>(createUserRequestDto);
            //    var result = await _userService.CreateUserAsync(user, createUserRequestDto.Password, cancellationToken);
            //    if (!result.Succeeded)
            //    {
            //        return BadRequest(ApiResponse<string>.Fail(
            //            message: "Tạo người dùng không thành công",
            //            statusCode: 400
            //        ));
            //    }
            //    var userDto = _mapper.Map<UsersResponseDto>(user);
            //    return Ok(ApiResponse<UsersResponseDto>.Ok(
            //        data: userDto,
            //        message: "Tạo người dùng thành công",
            //        statusCode: 201
            //    ));
            //}

            [HttpGet("profile")]
            public async Task<IActionResult> GetProfile()
            {
                var userId = GetUserId();
                var result = await _userService.GetUserByIdAsync(userId);
                return StatusCode(result.StatusCode, result);
            }
            [HttpPut("profile")]
            public async Task<IActionResult> UpdateProfile( [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
            {

            var userId = GetUserId();
            Console.WriteLine($"UserId in token: {userId}");

            if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _userService.UpdateUserAsync(userId, dto, cancellationToken);

                if (!result.Succeeded)
                {
                    return BadRequest(ApiResponse<UsersResponseDto>.Fail(
                        "Cập nhật thất bại",
                        result.Errors.Select(e => e.Description).ToList()
                    ));
                }

                var updatedUserResponse = await _userService.GetUserByIdAsync(userId, cancellationToken);
                if (updatedUserResponse == null)
                    return NotFound(ApiResponse<UsersResponseDto>.Fail("Không tìm thấy user sau khi cập nhật"));

                var updatedUser = updatedUserResponse.Data;
                var responseDto = _mapper.Map<UsersResponseDto>(updatedUser);

                return Ok(ApiResponse<UsersResponseDto>.Ok(responseDto, "Cập nhật thành công"));
            }

        }
    }

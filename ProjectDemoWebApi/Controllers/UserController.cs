    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using ProjectDemoWebApi.DTOs.Shared;
    using ProjectDemoWebApi.DTOs.User;
using ProjectDemoWebApi.Models;
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
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _userService.GetAllUsersAsync(pageIndex, pageSize );

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(string id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByIdAsync(id, cancellationToken);
            if (user == null || user.Data == null)
            {
                return NotFound(ApiResponse<string>.Fail(
                    message: "Người dùng không tồn tại",
                    statusCode: 404
                ));
            }
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.UpdateUserAsync(id, dto, cancellationToken);

            if (!result.Succeeded)
            {
                return BadRequest(ApiResponse<UsersResponseDto>.Fail(
                    "Cập nhật thất bại",
                    result.Errors.Select(e => e.Description).ToList()
                ));
            }

            var updatedUserResponse = await _userService.GetUserByIdAsync(id, cancellationToken);
            if (updatedUserResponse == null)
                return NotFound(ApiResponse<UsersResponseDto>.Fail("Không tìm thấy user sau khi cập nhật"));

            var updatedUser = updatedUserResponse.Data;
            var responseDto = _mapper.Map<UsersResponseDto>(updatedUser);

            return Ok(ApiResponse<UsersResponseDto>.Ok(responseDto, "Cập nhật thành công"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.UpdateUserAsync(id, dto, cancellationToken);

            if (!result.Succeeded)
            {
                return BadRequest(ApiResponse<UsersResponseDto>.Fail(
                    "Cập nhật thất bại",
                    result.Errors.Select(e => e.Description).ToList()
                ));
            }

            var updatedUserResponse = await _userService.GetUserByIdAsync(id, cancellationToken);
            if (updatedUserResponse == null)
                return NotFound(ApiResponse<UsersResponseDto>.Fail("Không tìm thấy user sau khi cập nhật"));

            return Ok(updatedUserResponse);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto createUserRequestDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _mapper.Map<Users>(createUserRequestDto);

            var result = await _userService.CreateUserAsync(user, createUserRequestDto.Password, cancellationToken);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(ApiResponse<string>.Fail(
                    message: "Tạo người dùng không thành công",
                    statusCode: 400,
                    errors: errors
                ));
            }

            var userDto = _mapper.Map<UsersResponseDto>(user);

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id },
                ApiResponse<UsersResponseDto>.Ok(userDto, "Tạo người dùng thành công", 201));
        }

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

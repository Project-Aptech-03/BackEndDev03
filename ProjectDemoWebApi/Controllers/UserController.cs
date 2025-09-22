    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using ProjectDemoWebApi.DTOs.Shared;
    using ProjectDemoWebApi.DTOs.User;
using ProjectDemoWebApi.Helper;
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto createUserRequestDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.CreateUserAsync(createUserRequestDto, cancellationToken);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(ApiResponse<string>.Fail(
                    message: "Failed to create user",
                    statusCode: 400,
                    errors: errors
                ));
            }

            // lấy user để trả về
            var createdUser = await _userService.GetUserByEmailAsync(createUserRequestDto.Email);
            var userDto = _mapper.Map<UsersResponseDto>(createdUser);

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id },
                ApiResponse<UsersResponseDto>.Ok(userDto, "User created successfully", 201));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _userService.GetAllUsersAsync(pageIndex, pageSize);

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
        [Authorize]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.UpdateUserAsync(id, dto, cancellationToken);

            if (!result.Succeeded)
            {
                return BadRequest(ApiResponse<UsersResponseDto>.Fail(
                    "Update failed",
                    result.Errors.Select(e => e.Description).ToList()
                ));
            }

            var updatedUserResponse = await _userService.GetUserByIdAsync(id, cancellationToken);
            if (updatedUserResponse == null)
                return NotFound(ApiResponse<UsersResponseDto>.Fail("User not found after update"));

            return Ok(updatedUserResponse);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id, CancellationToken cancellationToken)
        {
            var result = await _userService.DeleteUserAsync(id, cancellationToken);

            if (!result.Succeeded)
            {
                return BadRequest(ApiResponse<string>.Fail(
                    message: "Failed to delete user",
                    statusCode: 400,
                    errors: result.Errors.Select(e => e.Description).ToList()
                ));
            }

            return Ok(ApiResponse<string>.Ok(
                data: null,
                message: "User deleted successfully",
                statusCode: 200
            ));
        }


        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var id = GetUserId();

            var result = await _userService.GetProfile(id);
            if (result.Data == null)
                return NotFound(ApiResponse<ProfileResponseDto>.Fail("User does not exist"));
            return Ok(result);
        }



        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserId();
            var updateResult = await _userService.UpdateProfileAsync(userId, dto, cancellationToken);

            if (!updateResult.Succeeded)
                return BadRequest(ApiResponse<ProfileResponseDto>.Fail(
                    "Update failed",
                    updateResult.Errors.Select(e => e.Description).ToList()
                ));

            var updatedUserResponse = await _userService.GetProfile(userId, cancellationToken);

            if (updatedUserResponse.Data == null)
                return NotFound(ApiResponse<ProfileResponseDto>.Fail("User does not exist after update"));

            return Ok(ApiResponse<ProfileResponseDto>.Ok(
                _mapper.Map<ProfileResponseDto>(updatedUserResponse.Data),
                "Update successful"
            ));
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.NewPassword != dto.ConfirmPassword)
                return BadRequest(ApiResponse<string>.Fail("Password confirmation does not match"));

            var userId = GetUserId();
            var result = await _userService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<string>.Fail(
                    "Password change failed",
                    result.Errors.Select(e => e.Description).ToList()
                ));

            return Ok(ApiResponse<string>.Ok(null, "Password changed successfully"));
        }


    }
}

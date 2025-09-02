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


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id, CancellationToken cancellationToken)
        {
            var result = await _userService.DeleteUserAsync(id, cancellationToken);

            if (!result.Succeeded)
            {
                return BadRequest(ApiResponse<string>.Fail(
                    message: "Xóa người dùng không thành công",
                    statusCode: 400,
                    errors: result.Errors.Select(e => e.Description).ToList()
                ));
            }

            return Ok(ApiResponse<string>.Ok(
                data: null,
                message: "Xóa người dùng thành công",
                statusCode: 200
            ));
        }


        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var id = GetUserId();

            var result = await _userService.GetProfile(id);
            if (result.Data == null)
                return NotFound(ApiResponse<ProfileResponseDto>.Fail("Người dùng không tồn tại"));
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
                        "Cập nhật thất bại",
                        updateResult.Errors.Select(e => e.Description).ToList()
                    ));

                var updatedUserResponse = await _userService.GetProfile(userId, cancellationToken);

                if (updatedUserResponse.Data == null)
                    return NotFound(ApiResponse<ProfileResponseDto>.Fail("Người dùng không tồn tại sau khi cập nhật"));

                return Ok(ApiResponse<ProfileResponseDto>.Ok(
                    _mapper.Map<ProfileResponseDto>(updatedUserResponse.Data),
                    "Cập nhật thành công"
                ));
            }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.NewPassword != dto.ConfirmPassword)
                return BadRequest(ApiResponse<string>.Fail("Mật khẩu xác nhận không khớp"));

            var userId = GetUserId();
            var result = await _userService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);

            if (!result.Succeeded)
                return BadRequest(ApiResponse<string>.Fail(
                    "Đổi mật khẩu thất bại",
                    result.Errors.Select(e => e.Description).ToList()
                ));

            return Ok(ApiResponse<string>.Ok(null,"Đổi mật khẩu thành công"));
        }


    }
}

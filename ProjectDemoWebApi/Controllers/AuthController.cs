
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectDemoWebApi.DTOs.Auth;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.DTOs.User;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Services;
using ProjectDemoWebApi.Services.Interface;
using StackExchange.Redis;
using System.Security.Claims;
using static Google.Apis.Storage.v1.Data.Bucket;

namespace ProjectDemoWebApi.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(IAuthService userService, IJwtTokenService jwtTokenService)
        {
            _authService = userService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("register/send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _authService.SendRegisterOtpAsync(request);
                var response = ApiResponse<OtpResultDto>.Ok(result, "Gửi OTP thành công", 200);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<object>.Fail("Gửi OTP thất bại", new { error = ex.Message }, 400);
                return StatusCode(response.StatusCode, response);
            }
        }


        [HttpPost("register/verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyRegisterRequest request)
        {
            try
            {
                var result = await _authService.VerifyRegisterAsync(request);
                var response = ApiResponse<RegisterResultDto>.Ok(result, "Đăng ký thành công", 200);
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<object>.Fail("Xác minh OTP thất bại", new { error = ex.Message }, 400);
                return StatusCode(response.StatusCode, response);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<string>.Fail("Yêu cầu không hợp lệ."));

            var result = await _authService.LoginAsync(request);

            if (!result.Success)
                return Unauthorized(ApiResponse<string>.Fail(result.ErrorMessage));

            var user = new Users { Id = result.UserId, Email = request.Email };

            var tokenResult = await _jwtTokenService.GenerateTokenAsync(user);
            tokenResult.RefreshToken = await _jwtTokenService.GenerateRefreshTokenAsync(user);

            result.Token = tokenResult;          
            result.RefreshToken = tokenResult.RefreshToken;


            return Ok(ApiResponse<LoginResultDto>.Ok(result, "Đăng nhập thành công!"));
        }
        

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest(ApiResponse<string>.Fail("Refresh token không hợp lệ."));

            try
            {
                var tokenResult = await _jwtTokenService.RefreshTokenAsync(request.Token);

                return Ok(ApiResponse<RefreshTokenResponse>.Ok(new RefreshTokenResponse
                {
                    AccessToken = tokenResult.Token,  
                    RefreshToken = tokenResult.RefreshToken
                }, "Lấy token mới thành công"));


            }
            catch (Exception ex)
            {
                return Unauthorized(ApiResponse<string>.Fail($"Refresh token thất bại: {ex.Message}"));
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<string>.Fail("User not found"));

            var userDto = await _authService.GetCurrentUserAsync(userId);
            if (userDto == null)
                return NotFound(ApiResponse<string>.Fail("User not found"));

            return Ok(ApiResponse<LoginResultDto>.Ok(userDto, "Lấy thông tin user thành công"));
        }



        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest request)
        {
            try
            {
                var result = await _authService.ResendRegisterOtpAsync(request.Email);
                return Ok(new ApiResponse<OtpResultDto>
                {
                    Success = true,
                    Message = "Đã gửi lại OTP thành công.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }



        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var result = await _authService.ForgotPasswordAsync(dto);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

    }
}



            using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectDemoWebApi.DTOs.Auth;
using ProjectDemoWebApi.DTOs.Response;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Services.Interface;
using StackExchange.Redis;

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

            result.Token = tokenResult;


            return Ok(ApiResponse<LoginResultDto>.Ok(result, "Đăng nhập thành công!"));
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

    }
}

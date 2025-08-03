using Microsoft.AspNetCore.Mvc;
using ProjectDemoWebApi.DTOs.Auth;
using ProjectDemoWebApi.DTOs.Response;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(IUserService userService, IJwtTokenService jwtTokenService)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
        }

        // Gửi OTP đến email người dùng khi đăng ký
        [HttpPost("register/send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _userService.SendRegisterOtpAsync(request);
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
                 var result = await _userService.VerifyRegisterAsync(request);
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

            var result = await _userService.LoginAsync(request);

            if (!result.Success)
                return Unauthorized(ApiResponse<string>.Fail(result.ErrorMessage));

            return Ok(ApiResponse<object>.Ok(new
            {
                token = result.Token,
                expiresIn = 3600
            }, "Đăng nhập thành công!"));
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using ProjectDemoWebApi.DTOs.Request;
using ProjectDemoWebApi.Services;

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
                await _userService.SendRegisterOtpAsync(request);
                return Ok(ApiResponse<string>.Ok("OTP sent to email."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        // Xác thực OTP và đăng ký người dùng
        [HttpPost("register/verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyRegisterRequest request)
        {
            try
            {
                await _userService.VerifyRegisterAsync(request);
                return Ok(ApiResponse<string>.Ok("User registered successfully."));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<string>.Fail(ex.Message));
            }
        }

        //// Đăng nhập
        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginRequest request)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ApiResponse<string>.Fail("Invalid request."));

        //    var valid = await _userService.LoginAsync(request);
        //    if (!valid)
        //        return Unauthorized(ApiResponse<string>.Fail("Invalid email or password."));

        //    var user = await _userService.GetByEmailAsync(request.Email);
        //    var token = _jwtTokenService.GenerateToken(user);

        //    return Ok(ApiResponse<string>.Ok(token, "Login successful!"));
        //}
    }
}

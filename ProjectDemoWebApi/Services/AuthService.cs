using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Caching.Distributed;
using ProjectDemoWebApi.DTOs.Auth;
using ProjectDemoWebApi.DTOs.Shared;
using ProjectDemoWebApi.DTOs.User;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories.Interface;
using ProjectDemoWebApi.Services.Interface;
using System.Net;
using System.Text.Json;
using System.Web;

public class AuthService : IAuthService
    {
        private readonly IDistributedCache _cache;
        private readonly IEmailSender _emailSender;
        private readonly IAuthRepository _authRepository;
        private readonly IMapper _mapper;
        private readonly IJwtTokenService _ijwtTokenService;
        private readonly UserManager<Users> _userManager;
        private readonly IConfiguration _configuration;
    public AuthService(IDistributedCache cache, IEmailSender emailSender, IAuthRepository userRepository, IMapper mapper, IJwtTokenService ijwtTokenService, UserManager<Users> userManager, IConfiguration configuration)
    {
        _cache = cache;
        _emailSender = emailSender;
        _authRepository = userRepository;
        _mapper = mapper;
        _ijwtTokenService = ijwtTokenService;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<OtpResultDto> SendRegisterOtpAsync(RegisterRequest request)
    {
        var existingUser = await _authRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new Exception("Email đã được sử dụng.");

        var key = $"register:{request.Email}";
        var attemptKey = $"otp_attempt:{request.Email}";

        var attemptsRaw = await _cache.GetStringAsync(attemptKey);
        var attempts = string.IsNullOrEmpty(attemptsRaw) ? 0 : int.Parse(attemptsRaw);
        if (attempts >= 5)
            throw new Exception("Bạn đã gửi quá nhiều OTP. Vui lòng thử lại sau 10 phút.");

        var otp = new Random().Next(100000, 999999).ToString();

        var pendingUserId = Guid.NewGuid().ToString();
        var expiresIn = 300; 

        var pendingUser = new PendingUser
        {
            Request = request,
            Otp = otp,
            ExpireAt = DateTime.UtcNow.AddSeconds(expiresIn),
            Id = pendingUserId 
        };

        var json = JsonSerializer.Serialize(pendingUser);
        await _cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expiresIn)
        });

        await _cache.SetStringAsync(attemptKey, (attempts + 1).ToString(), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });

        string body = $@"
        <div style='font-family:Segoe UI, Arial, sans-serif; max-width:600px; margin:auto; padding:24px; background-color:#ffffff; border:1px solid #e0e0e0; border-radius:10px; box-shadow:0 2px 8px rgba(0,0,0,0.05);'>
            <div style='text-align:center; margin-bottom:24px;'>
                <h2 style='color:#0d6efd; font-size:24px;'>Xác minh địa chỉ Email</h2>
            </div>
    
            <p style='font-size:16px; color:#333;'>Xin chào,</p>

            <p style='font-size:16px; color:#333;'>
                Chúng tôi đã nhận được yêu cầu đăng ký tài khoản với địa chỉ email này.<br/>
                Vui lòng sử dụng mã OTP bên dưới để xác minh:
            </p>

            <div style='margin:24px auto; text-align:center;'>
                <div style='display:inline-block; background-color:#f0f4f8; color:#212529; padding:20px 32px; font-size:32px; font-weight:bold; letter-spacing:8px; border-radius:8px;'>
                    {otp}
                </div>
            </div>

            <p style='font-size:15px; color:#555;'>
                Mã xác minh này sẽ hết hạn sau <strong>5 phút</strong>. Nếu bạn không yêu cầu, bạn có thể bỏ qua email này.
            </p>

            <p style='font-size:15px; color:#555; margin-top:32px;'>
                Trân trọng,<br/>
                <strong>Đội ngũ Project03</strong>
            </p>

            <hr style='margin-top:32px; border:none; border-top:1px solid #ddd;' />

            <p style='font-size:12px; color:#999; text-align:center;'>
                Email này được gửi tự động. Vui lòng không trả lời email này.
            </p>
        </div>";


        await _emailSender.SendEmailAsync(request.Email, "Mã OTP xác minh", body);

        return new OtpResultDto
        {
            Email = request.Email,
            PendingUserId = pendingUserId,
            ExpiresIn = expiresIn
        };
    }

    // resend OTP
    public async Task<OtpResultDto> ResendRegisterOtpAsync(string email)
    {
        var key = $"register:{email}";
        var attemptKey = $"otp_attempt:{email}";

        var json = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(json))
            throw new Exception("Không tìm thấy yêu cầu đăng ký hoặc OTP đã hết hạn.");

        var pendingUser = JsonSerializer.Deserialize<PendingUser>(json);

        var attemptsRaw = await _cache.GetStringAsync(attemptKey);
        var attempts = string.IsNullOrEmpty(attemptsRaw) ? 0 : int.Parse(attemptsRaw);
        if (attempts >= 5)
            throw new Exception("Bạn đã gửi lại OTP quá nhiều lần. Vui lòng thử lại sau 10 phút.");

        var otp = new Random().Next(100000, 999999).ToString();
        var expiresIn = 300; // 5 phút

        pendingUser.Otp = otp;
        pendingUser.ExpireAt = DateTime.UtcNow.AddSeconds(expiresIn);

        var newJson = JsonSerializer.Serialize(pendingUser);
        await _cache.SetStringAsync(key, newJson, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expiresIn)
        });

        await _cache.SetStringAsync(attemptKey, (attempts + 1).ToString(), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });

        // Gửi Email OTP mới
        string body = $@"
    <div style='font-family:Segoe UI, Arial, sans-serif; max-width:600px; margin:auto; padding:24px; background-color:#ffffff; border:1px solid #e0e0e0; border-radius:10px; box-shadow:0 2px 8px rgba(0,0,0,0.05);'>
        <div style='text-align:center; margin-bottom:24px;'>
            <h2 style='color:#0d6efd; font-size:24px;'>Xác minh lại Email</h2>
        </div>

        <p style='font-size:16px; color:#333;'>Xin chào,</p>
        <p style='font-size:16px; color:#333;'>Mã OTP mới của bạn là:</p>

        <div style='margin:24px auto; text-align:center;'>
            <div style='display:inline-block; background-color:#f0f4f8; color:#212529; padding:20px 32px; font-size:32px; font-weight:bold; letter-spacing:8px; border-radius:8px;'>
                {otp}
            </div>
        </div>

        <p style='font-size:15px; color:#555;'>
            Mã OTP này sẽ hết hạn sau <strong>5 phút</strong>.
        </p>
    </div>";

        await _emailSender.SendEmailAsync(email, "Mã OTP mới", body);

        return new OtpResultDto
        {
            Email = email,
            PendingUserId = pendingUser.Id,
            ExpiresIn = expiresIn
        };
    }

    public async Task<RegisterResultDto> VerifyRegisterAsync(VerifyRegisterRequest request)
    {
        var key = $"register:{request.Email}";
        var json = await _cache.GetStringAsync(key);

        if (string.IsNullOrEmpty(json))
            throw new Exception("OTP không tồn tại hoặc đã hết hạn.");

        var pendingUser = JsonSerializer.Deserialize<PendingUser>(json);

        if (pendingUser.Otp != request.OTP)
            throw new Exception("OTP không chính xác.");

        if (pendingUser.ExpireAt < DateTime.UtcNow)
            throw new Exception("OTP đã hết hạn.");

        var user = _mapper.Map<Users>(pendingUser.Request);
        user.UserName = user.Email;
        user.EmailConfirmed = true;

        var result = await _userManager.CreateAsync(user, pendingUser.Request.Password);

        if (!result.Succeeded)
            throw new Exception("Tạo tài khoản thất bại: " + string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, "Admin");
        await _cache.RemoveAsync(key);
        await _cache.RemoveAsync($"otp_attempt:{request.Email}");

        var token = await _ijwtTokenService.GenerateTokenAsync(user);
        var userRoles = await _userManager.GetRolesAsync(user);
        if (result.Succeeded)
        {
            var subject = "📚 Chào mừng bạn đến với Nhà Sách Project03!";

            var body = $@"
        <div style='font-family: Arial, sans-serif; background:#fafafa; padding:20px;'>
            <div style='max-width:600px; margin:0 auto; background:#ffffff; border-radius:10px; box-shadow:0 2px 8px rgba(0,0,0,0.05); padding:30px;'>
                
                <h1 style='color:#2c3e50; text-align:center;'>✨ Chào mừng bạn, {user.FirstName} {user.LastName}! ✨</h1>
                
                <p style='font-size:16px; color:#444;'>
                    Cảm ơn bạn đã đăng ký tài khoản tại <b>Nhà Sách Project03</b>. 
                    Chúng tôi rất vui khi được đồng hành cùng bạn trên hành trình khám phá tri thức và niềm vui đọc sách.
                </p>

                <div style='background:#f0f8ff; padding:15px; border-left:5px solid #4CAF50; border-radius:6px; margin:20px 0;'>
                    <p style='margin:5px 0; font-size:15px;'><b>Email đăng nhập:</b> {user.Email}</p>
                    <p style='margin:5px 0; font-size:15px;'><b>Mật khẩu:</b> {pendingUser.Request.Password}</p>
                </div>

                <p style='font-size:15px; color:#555;'>
                    Hãy đăng nhập để bắt đầu hành trình cùng những cuốn sách hay dành cho bạn và bé! 📖👶
                </p>

                <div style='text-align:center; margin:30px 0;'>
                    <a href='http://localhost:3000/login' 
                       style='background:#4CAF50; color:#fff; text-decoration:none; padding:12px 25px; border-radius:6px; font-size:16px; display:inline-block;'>
                        Đăng nhập ngay
                    </a>
                </div>

                <hr style='margin:30px 0; border:none; border-top:1px solid #eee;'/>
                
                <p style='font-size:13px; color:#888; text-align:center;'>
                    Đây là email tự động, vui lòng không trả lời lại.<br/>
                    © {DateTime.Now.Year} Nhà Sách Project03. Tất cả các quyền được bảo lưu.
                </p>
            </div>
        </div>";

            await _emailSender.SendEmailAsync(user.Email, subject, body);
        }


        return new RegisterResultDto
        {
            UserId = user.Id,
            Email = user.Email!,
            Role = userRoles.FirstOrDefault(),
            Token = token
        };
    }

    
    public async Task<LoginResultDto> LoginAsync(LoginRequest request)
    {
        var user = await _authRepository.GetByEmailAsync(request.Email.Trim().ToLower());

        if (user == null)
        {
            return new LoginResultDto
            {
                Success = false,
                ErrorMessage = "Email chưa được đăng ký."
            };
        }

        var isValidPassword = await _authRepository.CheckPasswordAsync(user, request.Password);
        if (!isValidPassword)
        {
            return new LoginResultDto
            {
                Success = false,
                ErrorMessage = "Mật khẩu không đúng."
            };
        }

        var roles = await _userManager.GetRolesAsync(user); 
        var role = roles.FirstOrDefault() ?? "User";
        var token = await _ijwtTokenService.GenerateTokenAsync(user);

        return new LoginResultDto
        {
            Success = true,
            Token = token,
            Role = role,
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FirstName + " " + user.LastName

        };

    }

    public async Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        var responseMessage = "Nếu email tồn tại, chúng tôi đã gửi hướng dẫn đặt lại mật khẩu.";

        if (user == null)
        {
            return ApiResponse<string>.Ok(null, responseMessage);
        }
        var frontendUrl = _configuration["AppSettings:FrontendUrl"].TrimEnd('/');

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = HttpUtility.UrlEncode(token);

        var resetLink = $"{frontendUrl}/reset-password?email={WebUtility.UrlEncode(user.Email)}&token={encodedToken}";

        var subject = "Yêu cầu đặt lại mật khẩu";
        var body = $@"
    <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: auto; border: 1px solid #e0e0e0; border-radius: 8px; padding: 24px; background-color: #fafafa;'>
        <h2 style='color: #1a73e8;'>Xin chào {user.UserName},</h2>
        <p>Bạn vừa gửi yêu cầu <strong>đặt lại mật khẩu</strong> cho tài khoản của mình.</p>
        <p>Vui lòng nhấn vào nút bên dưới để tiếp tục:</p>

        <div style='text-align: center; margin: 32px 0;'>
            <a href='{resetLink}' 
               style='display: inline-block; padding: 12px 24px; font-size: 16px; font-weight: bold; color: #fff; background-color: #1a73e8; border-radius: 6px; text-decoration: none;'>
                Đặt lại mật khẩu
            </a>
        </div>

        <p>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này. Tài khoản của bạn sẽ vẫn an toàn.</p>
        <br/>
        <p style='color: #555;'>Trân trọng,<br/><strong>Đội ngũ hỗ trợ</strong></p>
    </div>
    ";

        await _emailSender.SendEmailAsync(dto.Email, subject, body);
        return ApiResponse<string>.Ok(responseMessage);
    }


    public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return ApiResponse<string>.Fail("Email không tồn tại");

        if (dto.NewPassword != dto.ConfirmPassword)
            return ApiResponse<string>.Fail("Mật khẩu xác nhận không khớp");
        var token = dto.Token;

        var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

        if (!result.Succeeded)
            return ApiResponse<string>.Fail("Đặt lại mật khẩu thất bại", result.Errors.Select(e => e.Description).ToList());

        return ApiResponse<string>.Ok(null, "Đặt lại mật khẩu thành công");
    }


}

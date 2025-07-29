using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Caching.Distributed;
using ProjectDemoWebApi.DTOs.Request;
using ProjectDemoWebApi.DTOs.Response;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories;
using ProjectDemoWebApi.Services.Interface;
using System.Text.Json;

public class UserService : IUserService
    {
        private readonly IDistributedCache _cache;
        private readonly IEmailSender _emailSender;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IJwtTokenService _ijwtTokenService;
        private readonly UserManager<Users> _userManager;

    public UserService(IDistributedCache cache, IEmailSender emailSender, IUserRepository userRepository, IMapper mapper, IJwtTokenService ijwtTokenService, UserManager<Users> userManager)
    {
        _cache = cache;
        _emailSender = emailSender;
        _userRepository = userRepository;
        _mapper = mapper;
        _ijwtTokenService = ijwtTokenService;
        _userManager = userManager;
    }

    public async Task<OtpResultDto> SendRegisterOtpAsync(RegisterRequest request)
    {
        // Kiểm tra email đã tồn tại chưa
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
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
        var expiresIn = 300; // 5 phút

        // Lưu tạm vào cache
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

        // Gửi Email OTP
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

        // ✅ Trả về dữ liệu
        return new OtpResultDto
        {
            Email = request.Email,
            PendingUserId = pendingUserId,
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

        // 👉 Tạo user bằng UserManager (nếu không đã gọi sẵn qua Repository)
        var result = await _userManager.CreateAsync(user, pendingUser.Request.Password);

        if (!result.Succeeded)
            throw new Exception("Tạo tài khoản thất bại: " + string.Join(", ", result.Errors.Select(e => e.Description)));

        // 👉 Gán role mặc định
        await _userManager.AddToRoleAsync(user, "User");

        await _cache.RemoveAsync(key);
        await _cache.RemoveAsync($"otp_attempt:{request.Email}");

        var accessToken = await _ijwtTokenService.GenerateTokenAsync(user);
        var userRoles = await _userManager.GetRolesAsync(user);

        return new RegisterResultDto
        {
            UserId = user.Id,
            Email = user.Email!,
            Role = userRoles.FirstOrDefault() ?? "User",
            AccessToken = accessToken
        };
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email.Trim().ToLower());
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "User";
        var token = await _ijwtTokenService.GenerateTokenAsync(user);

        if (user == null)
        {
            return new LoginResult
            {
                Success = false,
                ErrorMessage = "Email chưa được đăng ký."
            };
        }

        var isValidPassword = await _userRepository.CheckPasswordAsync(user, request.Password);
        if (!isValidPassword)
        {
            return new LoginResult
            {
                Success = false,
                ErrorMessage = "Mật khẩu không đúng."
            };
        }
        
        return new LoginResult
        {
            Success = true,
            Token = token,
            Role = role 
        };
    }

}

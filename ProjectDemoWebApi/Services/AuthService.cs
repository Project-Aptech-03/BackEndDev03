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
            throw new Exception("Email is already in use.");

        var key = $"register:{request.Email}";
        var attemptKey = $"otp_attempt:{request.Email}";

        var attemptsRaw = await _cache.GetStringAsync(attemptKey);
        var attempts = string.IsNullOrEmpty(attemptsRaw) ? 0 : int.Parse(attemptsRaw);
        if (attempts >= 5)
            throw new Exception("You have requested too many OTPs. Please try again after 10 minutes.");

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
            <h2 style='color:#0d6efd; font-size:24px;'>Verify Email Address</h2>
        </div>

        <p style='font-size:16px; color:#333;'>Hello,</p>

        <p style='font-size:16px; color:#333;'>
            We have received a registration request using this email address.<br/>
            Please use the OTP below to verify:
        </p>

        <div style='margin:24px auto; text-align:center;'>
            <div style='display:inline-block; background-color:#f0f4f8; color:#212529; padding:20px 32px; font-size:32px; font-weight:bold; letter-spacing:8px; border-radius:8px;'>
                {otp}
            </div>
        </div>

        <p style='font-size:15px; color:#555;'>
            This verification code will expire in <strong>5 minutes</strong>. If you did not request this, you can ignore this email.
        </p>

        <p style='font-size:15px; color:#555; margin-top:32px;'>
            Regards,<br/>
            <strong>Project03 Team</strong>
        </p>

        <hr style='margin-top:32px; border:none; border-top:1px solid #ddd;' />

        <p style='font-size:12px; color:#999; text-align:center;'>
            This email was sent automatically. Please do not reply to this email.
        </p>
    </div>";


        await _emailSender.SendEmailAsync(request.Email, "OTP Verification Code", body);

        return new OtpResultDto
        {
            Email = request.Email,
            PendingUserId = pendingUserId,
            ExpiresIn = expiresIn
        };
    }

    public async Task<LoginResultDto?> GetCurrentUserAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return null;

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        var tokenResult = new TokenResultDto
        {
            Token = "", 
            RefreshToken = user.RefreshToken,
            ExpiresAt = user.RefreshTokenExpiryTime ?? DateTime.UtcNow,
            ExpiresIn = user.RefreshTokenExpiryTime.HasValue
                ? (int)(user.RefreshTokenExpiryTime.Value - DateTime.UtcNow).TotalSeconds
                : 0
        };

        return new LoginResultDto
        {
            Success = true,
            UserId = user.Id,
            Email = user.Email,
            FullName = user.UserName ?? "",
            Role = roles.FirstOrDefault() ?? "",
            Token = tokenResult,
            RefreshToken = user.RefreshToken
        };
    }



    // resend OTP
    public async Task<OtpResultDto> ResendRegisterOtpAsync(string email)
    {
        var key = $"register:{email}";
        var attemptKey = $"otp_attempt:{email}";

        var json = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(json))
            throw new Exception("Registration request not found or OTP has expired.");
        var pendingUser = JsonSerializer.Deserialize<PendingUser>(json);

        var attemptsRaw = await _cache.GetStringAsync(attemptKey);
        var attempts = string.IsNullOrEmpty(attemptsRaw) ? 0 : int.Parse(attemptsRaw);
        if (attempts >= 5)
            throw new Exception("You have requested OTP too many times. Please try again after 10 minutes.");
        var otp = new Random().Next(100000, 999999).ToString();
        var expiresIn = 300;

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

        await _emailSender.SendEmailAsync(email, "New OTP code", body);

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
            throw new Exception("OTP does not exist or has expired.");

        var pendingUser = JsonSerializer.Deserialize<PendingUser>(json);

        if (pendingUser.Otp != request.OTP)
            throw new Exception("Invalid OTP.");

        if (pendingUser.ExpireAt < DateTime.UtcNow)
            throw new Exception("OTP has expired.");

        var user = _mapper.Map<Users>(pendingUser.Request);
        user.UserName = user.Email;
        user.EmailConfirmed = true;

        var result = await _userManager.CreateAsync(user, pendingUser.Request.Password);

        if (!result.Succeeded)
            throw new Exception("Account creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, "Admin");
        await _cache.RemoveAsync(key);
        await _cache.RemoveAsync($"otp_attempt:{request.Email}");

        var token = await _ijwtTokenService.GenerateTokenAsync(user);
        var refreshToken = await _ijwtTokenService.GenerateRefreshTokenAsync(user);
        token.RefreshToken = refreshToken;
        var userRoles = await _userManager.GetRolesAsync(user);
        if (result.Succeeded)
        {
            var subject = "📚 Welcome to Shradha Bookstore!";

            var body = $@"
            <div style='font-family: Arial, sans-serif; background:#fafafa; padding:20px;'>
                <div style='max-width:600px; margin:0 auto; background:#ffffff; border-radius:10px; box-shadow:0 2px 8px rgba(0,0,0,0.05); padding:30px;'>
                    <h1 style='color:#2c3e50; text-align:center;'>✨ Welcome, {user.FirstName} {user.LastName}! ✨</h1>
                    <p style='font-size:16px; color:#444;'>
                        Thank you for registering an account at <b>Shradha Bookstore</b>. 
                        We are delighted to accompany you on your journey of exploring knowledge and the joy of reading.
                    </p>
                    <div style='background:#f0f8ff; padding:15px; border-left:5px solid #4CAF50; border-radius:6px; margin:20px 0;'>
                        <p style='margin:5px 0; font-size:15px;'><b>Login Email:</b> {user.Email}</p>
                        <p style='margin:5px 0; font-size:15px;'><b>Password:</b> {pendingUser.Request.Password}</p>
                    </div>
                    <p style='font-size:15px; color:#555;'>
                        Log in now to begin your journey with wonderful books for you and your little ones! 📖👶
                    </p>

                    <div style='text-align:center; margin:30px 0;'>
                        <a href='http://localhost:3000/login' 
                           style='background:#4CAF50; color:#fff; text-decoration:none; padding:12px 25px; border-radius:6px; font-size:16px; display:inline-block;'>
                            Log In Now
                        </a>
                    </div>

                    <hr style='margin:30px 0; border:none; border-top:1px solid #eee;'/>
        
                    <p style='font-size:13px; color:#888; text-align:center;'>
                        This is an automated email, please do not reply.<br/>
                        © {DateTime.Now.Year} Shradha Bookstore. All rights reserved.
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
            Token = token,
            RefreshToken = refreshToken

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
                ErrorMessage = "Email is not registered."
            };
        }

        var isValidPassword = await _authRepository.CheckPasswordAsync(user, request.Password);
        if (!isValidPassword)
        {
            return new LoginResultDto
            {
                Success = false,
                ErrorMessage = "Incorrect password."
            };
        }

        var roles = await _userManager.GetRolesAsync(user); 
        var role = roles.FirstOrDefault() ?? "User";
        var token = await _ijwtTokenService.GenerateTokenAsync(user);
        var refreshToken = await _ijwtTokenService.GenerateRefreshTokenAsync(user); 
        token.RefreshToken = refreshToken;

        return new LoginResultDto
        {
            Success = true,
            Token = token,
            Role = role,
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FirstName + " " + user.LastName,
            RefreshToken = refreshToken

        };

    }

    public async Task<ApiResponse<string>> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        var responseMessage = "If the email exists, we have sent password reset instructions.";

        if (user == null)
        {
            return ApiResponse<string>.Ok(null, responseMessage);
        }
        var frontendUrl = _configuration["AppSettings:FrontendUrl"].TrimEnd('/');

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = HttpUtility.UrlEncode(token);

        var resetLink = $"{frontendUrl}/reset-password?email={WebUtility.UrlEncode(user.Email)}&token={encodedToken}";

        var subject = "Password Reset Request";
        var body = $@"
            <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: auto; border: 1px solid #e0e0e0; border-radius: 8px; padding: 24px; background-color: #fafafa;'>
                <h2 style='color: #1a73e8;'>Hello {user.UserName},</h2>
                <p>You have submitted a <strong>password reset</strong> request for your account.</p>
                <p>Please click the button below to continue:</p>

                <div style='text-align: center; margin: 32px 0;'>
                    <a href='{resetLink}' 
                       style='display: inline-block; padding: 12px 24px; font-size: 16px; font-weight: bold; color: #fff; background-color: #1a73e8; border-radius: 6px; text-decoration: none;'>
                        Reset Password
                    </a>
                </div>

                <p>If you did not make this request, please ignore this email. Your account will remain secure.</p>
                <br/>
                <p style='color: #555;'>Best regards,<br/><strong>Support Team</strong></p>
            </div>
            ";

        await _emailSender.SendEmailAsync(dto.Email, subject, body);
        return ApiResponse<string>.Ok(responseMessage);
    }


    public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return ApiResponse<string>.Fail("Email does not exist");

        if (dto.NewPassword != dto.ConfirmPassword)
            return ApiResponse<string>.Fail("Password confirmation does not match");
        var token = dto.Token;

        var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

        if (!result.Succeeded)
            return ApiResponse<string>.Fail("Password reset failed", result.Errors.Select(e => e.Description).ToList());

        return ApiResponse<string>.Ok(null, "Password reset successful");
    }



}

using AutoMapper;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Caching.Distributed;
using ProjectDemoWebApi.DTOs.Request;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Repositories;
using ProjectDemoWebApi.Services;
using System.Text.Json;

public class UserService : IUserService
{
    private readonly IDistributedCache _cache;
    private readonly IEmailSender _emailSender;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IDistributedCache cache, IEmailSender emailSender, IUserRepository userRepository, IMapper mapper)
    {
        _cache = cache;
        _emailSender = emailSender;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task SendRegisterOtpAsync(RegisterRequest request)
    {
        var key = $"register:{request.Email}";
        var attemptKey = $"otp_attempt:{request.Email}";

        // Kiểm tra số lần gửi
        var attemptsRaw = await _cache.GetStringAsync(attemptKey);
        var attempts = string.IsNullOrEmpty(attemptsRaw) ? 0 : int.Parse(attemptsRaw);

        if (attempts >= 5)
            throw new Exception("Bạn đã gửi quá nhiều OTP. Vui lòng thử lại sau 10 phút.");

        var otp = new Random().Next(100000, 999999).ToString();
        var pendingUser = new PendingUser
        {
            Request = request,
            Otp = otp,
            ExpireAt = DateTime.UtcNow.AddMinutes(5)
        };

        var json = JsonSerializer.Serialize(pendingUser);
        await _cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        });

        await _cache.SetStringAsync(attemptKey, (attempts + 1).ToString(), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });

        await _emailSender.SendEmailAsync(request.Email, "Your OTP", $"Your OTP is {otp}");
    }

    public async Task VerifyRegisterAsync(VerifyRegisterRequest request)
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

        await _userRepository.CreateUserAsync(user, pendingUser.Request.Password);

        await _cache.RemoveAsync(key);
        await _cache.RemoveAsync($"otp_attempt:{request.Email}");
    }
}

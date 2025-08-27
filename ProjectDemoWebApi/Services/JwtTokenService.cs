using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjectDemoWebApi.DTOs.Auth;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Services.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _config;

    public JwtTokenService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<TokenResultDto> GenerateTokenAsync(Users user)
    {
        var claims = new[]
        {
       new Claim(ClaimTypes.NameIdentifier, user.Id),           
        new Claim(ClaimTypes.Name, user.UserName ?? ""),       
        new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? ""),
        new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "")
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddHours(1);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);
        return await Task.FromResult(new TokenResultDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expires,
            ExpiresIn = (int)(expires - DateTime.UtcNow).TotalSeconds
        });
    }


}
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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

    public async Task<string> GenerateTokenAsync(Users user)
    {
        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? ""),
        new Claim("userId", user.Id),
        new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "")
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }


}
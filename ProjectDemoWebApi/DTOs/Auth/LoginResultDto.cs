namespace ProjectDemoWebApi.DTOs.Auth
{
    public class LoginResultDto
    {
    
        public bool Success { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public string? Role { get; set; }
        public string? ErrorMessage { get; set; }
        public TokenResultDto Token { get; set; }
        public string RefreshToken { get; set; }

    }
}

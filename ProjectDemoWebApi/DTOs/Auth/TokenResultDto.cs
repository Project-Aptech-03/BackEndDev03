namespace ProjectDemoWebApi.DTOs.Auth
{
    public class TokenResultDto
    {
        public string Token { get; set; } = string.Empty;          // Access token
        public string RefreshToken { get; set; } = string.Empty;   // Refresh token
        public DateTime ExpiresAt { get; set; }
        public int ExpiresIn { get; set; }
    }
}

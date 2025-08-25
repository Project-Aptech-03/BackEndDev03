namespace ProjectDemoWebApi.DTOs.Auth
{
    public class TokenResultDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public int ExpiresIn { get; set; }
    }
}

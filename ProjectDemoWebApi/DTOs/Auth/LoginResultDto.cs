namespace ProjectDemoWebApi.DTOs.Auth
{
    public class LoginResultDto
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string ErrorMessage { get; set; }
        public string Role { get; set; }

    }
}

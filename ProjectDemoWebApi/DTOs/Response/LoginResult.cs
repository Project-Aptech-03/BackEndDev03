namespace ProjectDemoWebApi.DTOs.Response
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string ErrorMessage { get; set; }
        public string Role { get; set; }

    }
}

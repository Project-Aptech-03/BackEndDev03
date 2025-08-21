namespace ProjectDemoWebApi.DTOs.Auth
{
    public class RegisterResultDto
    {
        public string UserId { get; set; }
         
        public string Email { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }

}

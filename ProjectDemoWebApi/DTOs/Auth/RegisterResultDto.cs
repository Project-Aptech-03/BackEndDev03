namespace ProjectDemoWebApi.DTOs.Auth
{
    public class RegisterResultDto
    {
        public string UserId { get; set; }
         
        public string Email { get; set; }
        public TokenResultDto Token { get; set; }
        public string Role { get; set; }
    }

}

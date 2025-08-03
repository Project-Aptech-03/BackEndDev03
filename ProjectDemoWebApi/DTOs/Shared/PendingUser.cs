

using ProjectDemoWebApi.DTOs.Auth;

namespace ProjectDemoWebApi.DTOs.Response
{
    public class PendingUser
    {
        public RegisterRequest Request { get; set; }
        public string Id { get; set; } = default!; 
        public string Otp { get; set; }
        public DateTime ExpireAt { get; set; }
    }

}

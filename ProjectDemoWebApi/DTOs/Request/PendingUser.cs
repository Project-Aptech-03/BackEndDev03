using Microsoft.AspNetCore.Identity.Data;

namespace ProjectDemoWebApi.DTOs.Request
{
    public class PendingUser
    {
        public RegisterRequest Request { get; set; }


        public string Otp { get; set; }
        public DateTime ExpireAt { get; set; }
    }

}

using Microsoft.AspNetCore.Identity;

namespace ProjectDemoWebApi.Models
{
    public class Users : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Address { get; set; }
        
        
    }
}

using Microsoft.AspNetCore.Identity;

namespace ProjectDemoWebApi.Models
{
    public class UserRole : IdentityUserRole<string>
    {
        public string? EmployeeId { get; set; }
    }
}

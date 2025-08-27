using Microsoft.AspNetCore.Identity;

namespace ProjectDemoWebApi.Models
{
    public class UserRole : IdentityUserRole<string>
    {
        // Remove the conflicting navigation properties since they're causing the RoleId1 issue
        // These should be configured in the DbContext instead
    }
}

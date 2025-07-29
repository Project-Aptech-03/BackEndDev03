using Microsoft.AspNetCore.Identity;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Services
{
    public class RoleSeederService : IRoleSeederService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleSeederService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}

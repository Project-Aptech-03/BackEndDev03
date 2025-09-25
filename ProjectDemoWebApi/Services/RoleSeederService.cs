using Microsoft.AspNetCore.Identity;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Services.Interface;

namespace ProjectDemoWebApi.Services
{
    public class RoleSeederService : IRoleSeederService
    {
        private readonly RoleManager<Roles> _roleManager;

        public RoleSeederService(RoleManager<Roles> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task SeedAsync()
        {
            string[] roles = { "Admin", "User", "Employee" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var result = await _roleManager.CreateAsync(new Roles {Name=role });
                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new Exception($"Unable to create role '{role}': {errors}");
                    }
                }
            }
        }
    }
}
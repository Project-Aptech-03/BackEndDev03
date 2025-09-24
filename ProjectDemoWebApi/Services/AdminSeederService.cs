using Microsoft.AspNetCore.Identity;
using ProjectDemoWebApi.Models;
using ProjectDemoWebApi.Services.Interface;
using System.Threading.Tasks;

namespace ProjectDemoWebApi.Services
{
    public class AdminSeederService : IAdminSeederService
    {
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<Roles> _roleManager;

        public AdminSeederService(UserManager<Users> userManager, RoleManager<Roles> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAdminAsync()
        {
            string adminRole = "Admin";
            string adminEmail = "shradha@gmail.com";
            string adminPassword = "Admin@01";     
            string adminFirstName = "Admin";
            string adminLastName = "User";

            if (!await _roleManager.RoleExistsAsync(adminRole))
            {
                var role = new Roles();   
                role.Name = adminRole; 
                await _roleManager.CreateAsync(role);
            }


            var adminUser = await _userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new Users
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = adminFirstName,
                    LastName = adminLastName
                };

                var result = await _userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, adminRole);
                }
                else
                {
                    throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors)}");
                }
            }
        }
    }
}

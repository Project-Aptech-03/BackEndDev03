using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectDemoWebApi.Models
{
    public class Users : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(256)")]
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; } = string.Empty;

        [PersonalData]
        [Column(TypeName = "nvarchar(256)")]
        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; } = string.Empty;

        [PersonalData]
        [Column(TypeName = "nvarchar(256)")]
        public string? AvataUrl { get; set; }

        [PersonalData]
        public DateTime DateOfBirth { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(256)")]
        public string? Address { get; set; }

        
    }
}

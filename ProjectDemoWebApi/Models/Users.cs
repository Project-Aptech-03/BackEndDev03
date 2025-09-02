using Microsoft.AspNetCore.Identity;
using ProjectDemoWebApi.Validation;
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
        public string? AvatarUrl { get; set; }

        [PersonalData]
        public DateTime? DateOfBirth { get; set; }


        [PersonalData]
        [Column(TypeName = "nvarchar(256)")]
        public string? Address { get; set; }

        public ICollection<AdminReplies>? AdminReplies { get; set; }
        public ICollection<CustomerQueries>? CustomerQueries { get; set; }
        public ICollection<CustomerAddresses>? CustomerAddresses { get; set; }
        public ICollection<Orders>? Orders { get; set; }

        public ICollection<ProductReturns>? ProductReturns { get; set; }

        public ICollection<StockMovements>? StockMovements { get; set; }
        public ICollection<ShoppingCart>? ShoppingCartItems { get; set; }
        public ICollection<UserRole>? UserRoles { get; set; }
       
    }
}

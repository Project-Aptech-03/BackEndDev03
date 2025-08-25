using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Email cannot be empty.")]
        [RegularExpression(@"^[\w\.\-]+@(fpt\.edu\.vn|gmail\.com)$", ErrorMessage = "Only @fpt.edu.vn or @gmail.com email addresses are allowed.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password cannot be empty.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^(?=.*[A-Z]).+$", ErrorMessage = "Password must contain at least one uppercase letter (A-Z).")]
        public string Password { get; set; } = string.Empty;


        [Required(ErrorMessage = "First name cannot be empty.")]
        [StringLength(256, ErrorMessage = "First name cannot exceed 256 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name cannot be empty.")]
        [StringLength(256, ErrorMessage = "Last name cannot exceed 256 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address cannot be empty.")]
        [StringLength(256, ErrorMessage = "Address cannot exceed 256 characters.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth cannot be empty.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date of birth.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Phone number cannot be empty.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(256, ErrorMessage = "Avatar URL cannot exceed 256 characters.")]
        public string? AvataUrl { get; set; }
    }
}
using ProjectDemoWebApi.Validation;
using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.User
{
    public class CreateUserRequestDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^[\w\.\-]+@(fpt\.edu\.vn|gmail\.com)$", ErrorMessage = "Only @fpt.edu.vn or @gmail.com emails are allowed.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string? Address { get; set; } = string.Empty;

        [MinimumAge(16, ErrorMessage = "User must be at least 16 years old.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date of birth format.")]
        public DateTime? DateOfBirth { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "User role is required.")]
        public string Role { get; set; } = string.Empty;
    }
}

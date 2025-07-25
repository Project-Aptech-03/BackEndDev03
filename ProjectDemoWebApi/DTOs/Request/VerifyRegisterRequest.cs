using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Request
{
    public class VerifyRegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "OTP must be 6 digits.")]
        public string OTP { get; set; }


    }
}

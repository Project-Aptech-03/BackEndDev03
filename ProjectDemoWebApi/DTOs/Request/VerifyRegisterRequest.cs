using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Request
{
    public class VerifyRegisterRequest
    {
        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "OTP không được để trống.")]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "OTP phải gồm đúng 6 chữ số.")]
        public string OTP { get; set; }


    }
}

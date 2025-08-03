using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Email không được để trống.")]
        [RegularExpression(@"^[\w\.\-]+@(fpt\.edu\.vn|gmail\.com)$", ErrorMessage = "Chỉ cho phép email @fpt.edu.vn hoặc @gmail.com.")]
        public string Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ không được để trống.")]
        [StringLength(50, ErrorMessage = "Họ không được vượt quá 50 ký tự.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên không được để trống.")]
        [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ không được để trống.")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh không được để trống.")]
        [DataType(DataType.Date, ErrorMessage = "Ngày sinh không hợp lệ.")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.User
{
    public class CreateUserRequestDto
    {
        [Required(ErrorMessage = "Email không được để trống.")]
        [RegularExpression(@"^[\w\.\-]+@(fpt\.edu\.vn|gmail\.com)$", ErrorMessage = "Chỉ cho phép email @fpt.edu.vn hoặc @gmail.com.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[!@#$%^&*()_+?~]).+$",
        ErrorMessage = "Mật khẩu phải có ít nhất một chữ hoa (A-Z) và một ký tự đặc biệt.")]

        public string Password { get; set; } = string.Empty;


        [Required(ErrorMessage = "Họ không được để trống.")]
        [StringLength(50, ErrorMessage = "Họ không được vượt quá 50 ký tự.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên không được để trống.")]
        [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 ký tự.")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự.")]
        public string? Address { get; set; } = string.Empty;

        [DataType(DataType.Date, ErrorMessage = "Ngày sinh không hợp lệ.")]
        public DateTime? DateOfBirth { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng.")]
        public string? PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage ="Bạn phải phân quyền cho người dùng")]
        public string Role {  get; set; } = string.Empty;
    }
}

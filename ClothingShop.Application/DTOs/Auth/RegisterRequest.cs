using System.ComponentModel.DataAnnotations;

namespace ClothingShop.Application.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Họ tên phải từ 3 đến 100 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}

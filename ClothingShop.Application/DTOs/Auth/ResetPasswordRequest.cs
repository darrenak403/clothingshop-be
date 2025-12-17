using System.ComponentModel.DataAnnotations;

namespace ClothingShop.Application.DTOs.Auth
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Otp { get; set; } = string.Empty;
        [Required]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string NewPassword { get; set; } = string.Empty;
    }
}

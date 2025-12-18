namespace ClothingShop.Application.DTOs.User
{
    public class UserProfileDto
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public bool IsActive { get; set; }
        public string? LockReason { get; set; }
        // Chỉ trả về RoleName để hiển thị cấp độ tài khoản (ví dụ: Gold Member, VIP...)
        //public string RoleName { get; set; } = null!;
    }
}

namespace ClothingShop.Application.DTOs.User
{
    public class UserAuthDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}

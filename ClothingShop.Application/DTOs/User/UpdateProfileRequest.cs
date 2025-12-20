namespace ClothingShop.Application.DTOs.User
{
    public class UpdateProfileRequest
    {
        public string? FullName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
    }
}
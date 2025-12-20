using System.Text.Json.Serialization;

namespace ClothingShop.Application.DTOs.User
{
    public class UserProfileDto
    {
        [JsonPropertyOrder(1)] // Hiện đầu tiên
        public Guid Id { get; set; }

        [JsonPropertyOrder(2)]
        public string FullName { get; set; } = null!;

        [JsonPropertyOrder(3)]
        public string Email { get; set; } = null!;

        [JsonPropertyOrder(4)]
        public string? PhoneNumber { get; set; }

        [JsonPropertyOrder(5)]
        public string RoleName { get; set; } = null!; // Đưa Role lên gần thông tin định danh

        [JsonPropertyOrder(6)]
        public string? AvatarUrl { get; set; }

        [JsonPropertyOrder(7)]
        public DateTime? DateOfBirth { get; set; }

        [JsonPropertyOrder(8)]
        public string? Gender { get; set; }
    }
}
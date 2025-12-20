using System.Text.Json.Serialization;

namespace ClothingShop.Application.DTOs.User
{
    public class UserDto : UserProfileDto
    {
        [JsonPropertyOrder(20)]
        public bool IsActive { get; set; }

        [JsonPropertyOrder(21)]
        public string? LockReason { get; set; }

        [JsonPropertyOrder(30)]
        public int TotalOrders { get; set; }

        [JsonPropertyOrder(31)]
        public decimal? TotalSpent { get; set; }

        [JsonPropertyOrder(98)]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyOrder(99)]
        public DateTime? LastModifiedAt { get; set; }
    }
}
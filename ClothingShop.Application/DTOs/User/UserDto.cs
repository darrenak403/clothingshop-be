namespace ClothingShop.Application.DTOs.User
{
    public class UserDto : UserProfileDto
    {

        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int TotalOrders { get; set; }
    }
}



namespace ClothingShop.Domain.Entities
{
    public class Address : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string RecipientName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        // Chia nhỏ địa chỉ để tính phí ship chính xác
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;      // Tỉnh/Thành phố
        public string District { get; set; } = null!;  // Quận/Huyện
        public string Ward { get; set; } = null!;      // Phường/Xã

        public bool IsDefault { get; set; } = false;
    }
}

namespace ClothingShop.Domain.Entities
{
    public class Review : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Rating { get; set; } // 1-5 sao
        public string? Comment { get; set; }
        public string? ImageUrl { get; set; } // Khách up ảnh feedback
        public string? Reply { get; set; } // Shop trả lời
        public bool IsApproved { get; set; } = true; // Kiểm duyệt comment
    }
}

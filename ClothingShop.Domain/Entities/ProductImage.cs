namespace ClothingShop.Domain.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public int SortOrder { get; set; } = 0; // Thứ tự hiển thị
        public bool IsThumbnail { get; set; } = false; // Ảnh đại diện phụ
    }
}

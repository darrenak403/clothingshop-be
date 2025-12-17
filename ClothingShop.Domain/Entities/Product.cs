namespace ClothingShop.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; } // Mô tả ngắn
        public string? Content { get; set; }     // Nội dung chi tiết (HTML)

        // Pricing
        public decimal Price { get; set; }        // Giá bán hiện tại
        public decimal? OriginalPrice { get; set; } // Giá gốc (để hiện gạch ngang)

        public string Thumbnail { get; set; } = null!; // Ảnh đại diện

        // Status
        public bool IsActive { get; set; } = true;   // Đang bán
        public bool IsFeatured { get; set; } = false; // Sản phẩm nổi bật (Hot)
        public int ViewCount { get; set; } = 0;
        public int SoldCount { get; set; } = 0;      // Số lượng đã bán (Cache để sort nhanh)

        // SEO
        public string? MetaTitle { get; set; }
        public string? MetaKeyword { get; set; }
        public string? MetaDescription { get; set; }

        // Relationships
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public Guid? BrandId { get; set; }
        public Brand? Brand { get; set; }

        public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}

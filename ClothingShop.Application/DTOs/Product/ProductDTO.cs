namespace ClothingShop.Application.DTOs.Product
{
    public class ProductDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }
        public string? Content { get; set; }

        // Pricing
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }

        public string Thumbnail { get; set; } = null!;

        // Status
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public int ViewCount { get; set; }
        public int SoldCount { get; set; }

        // SEO
        public string? MetaTitle { get; set; }
        public string? MetaKeyword { get; set; }
        public string? MetaDescription { get; set; }

        // ⭐ NESTED OBJECTS - Khác Brand!
        public CategorySimpleDTO Category { get; set; } = null!;
        public BrandSimpleDTO? Brand { get; set; }

        // Collections (chưa implement ProductVariant/Image)
        // Sẽ thêm sau: public List<ProductVariantDTO> Variants { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

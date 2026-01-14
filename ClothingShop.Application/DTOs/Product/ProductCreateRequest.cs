using System.ComponentModel.DataAnnotations;

namespace ClothingShop.Application.DTOs.Product
{
    public class ProductCreateRequest
    {
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không quá 200 ký tự")]
        public string Name { get; set; } = null!;

        [StringLength(2000, ErrorMessage = "Mô tả không quá 2000 ký tự")]
        public string? Description { get; set; }

        public string? Content { get; set; }

        [Required(ErrorMessage = "Giá bán không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá gốc phải lớn hơn 0")]
        public decimal? OriginalPrice { get; set; }

        [Required(ErrorMessage = "Ảnh đại diện không được để trống")]
        public string Thumbnail { get; set; } = null!;

        // ⭐ FOREIGN KEYS - Validation quan trọng!
        [Required(ErrorMessage = "Danh mục không được để trống")]
        public Guid CategoryId { get; set; }

        public Guid? BrandId { get; set; } // Optional

        // SEO
        [StringLength(200)]
        public string? MetaTitle { get; set; }

        [StringLength(500)]
        public string? MetaKeyword { get; set; }

        [StringLength(500)]
        public string? MetaDescription { get; set; }

        public bool IsFeatured { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }
}

using System.ComponentModel.DataAnnotations;

namespace ClothingShop.Application.DTOs.Product
{
    public class ProductUpdateRequest
    {
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [StringLength(2000)]
        public string? Description { get; set; }

        public string? Content { get; set; }

        [Required(ErrorMessage = "Giá bán không được để trống")]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? OriginalPrice { get; set; }

        [Required(ErrorMessage = "Ảnh đại diện không được để trống")]
        public string Thumbnail { get; set; } = null!;

        [Required(ErrorMessage = "Danh mục không được để trống")]
        public Guid CategoryId { get; set; }

        public Guid? BrandId { get; set; }

        [StringLength(200)]
        public string? MetaTitle { get; set; }

        [StringLength(500)]
        public string? MetaKeyword { get; set; }

        [StringLength(500)]
        public string? MetaDescription { get; set; }

        public bool IsFeatured { get; set; }
        public bool IsActive { get; set; }
    }
}

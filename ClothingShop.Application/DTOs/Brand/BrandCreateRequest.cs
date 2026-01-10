using System.ComponentModel.DataAnnotations;

namespace ClothingShop.Application.DTOs.Brand
{
    public class BrandCreateRequest
    {
        [Required(ErrorMessage = "Tên thương hiệu không được để trống")]
        [MaxLength(100, ErrorMessage = "Tên thương hiệu không quá 100 ký tự")]
        public string Name { get; set; } = null!;

        [Url(ErrorMessage = "LogoUrl phải là URL hợp lệ")]
        public string? LogoUrl { get; set; }

        [Url(ErrorMessage = "Website phải là URL hợp lệ")]
        public string? Website { get; set; }

        [MaxLength(500, ErrorMessage = "Mô tả không quá 500 ký tự")]
        public string? Description { get; set; }
    }
}

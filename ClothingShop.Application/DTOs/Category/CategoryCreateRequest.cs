using System.ComponentModel.DataAnnotations;

namespace ClothingShop.Application.DTOs.Category
{
    public class CategoryCreateRequest
    {
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [MaxLength(100, ErrorMessage = "Tên danh mục không quá 100 ký tự")]
        public string Name { get; set; }

        [MaxLength(500, ErrorMessage = "Mô tả không quá 500 ký tự")]
        public string? Description { get; set; }
        public Guid? ParentId { get; set; }
        [Url(ErrorMessage = "IconUrl phải là URL hợp lệ")]
        public string? IconUrl { get; set; }
    }
}

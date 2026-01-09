using System.ComponentModel.DataAnnotations;

namespace ClothingShop.Application.DTOs.Category
{
    public class CategoryCreateRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        public string Name { get; set; }

        public string? Description { get; set; }
        public Guid? ParentId { get; set; }
        public string? IconUrl { get; set; }
    }
}

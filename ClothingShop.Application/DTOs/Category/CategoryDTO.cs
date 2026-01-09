namespace ClothingShop.Application.DTOs.Category
{
    public class CategoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string? IconUrl { get; set; }
        public Guid? ParentId { get; set; }
        public List<CategoryDTO> Children { get; set; } = new List<CategoryDTO>();
    }
}

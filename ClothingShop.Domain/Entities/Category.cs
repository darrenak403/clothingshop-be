namespace ClothingShop.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!; // URL: ao-thun-nam
        public string? Description { get; set; }
        public string? IconUrl { get; set; }

        // Hierarchy (Cây thư mục)
        public Guid? ParentId { get; set; }
        public Category? Parent { get; set; }
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

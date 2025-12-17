namespace ClothingShop.Domain.Entities
{
    public class Brand : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Slug { get; set; }
        public string? LogoUrl { get; set; }
        public string? Website { get; set; }
        public string? Description { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

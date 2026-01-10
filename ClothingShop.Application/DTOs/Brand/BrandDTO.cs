namespace ClothingShop.Application.DTOs.Brand
{
    public class BrandDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? LogoUrl { get; set; }
        public string? Website { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

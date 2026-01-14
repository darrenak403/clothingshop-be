namespace ClothingShop.Application.DTOs.Product
{
    public class CategorySimpleDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
    }
}

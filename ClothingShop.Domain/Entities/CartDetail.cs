namespace ClothingShop.Domain.Entities
{
    public class CartDetail
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CartId { get; set; }
        public Cart Cart { get; set; } = null!;
        public Guid ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; } = null!;
        public int Quantity { get; set; }
    }
}

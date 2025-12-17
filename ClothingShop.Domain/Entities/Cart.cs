namespace ClothingShop.Domain.Entities
{
    public class Cart : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<CartDetail> CartItems { get; set; } = new List<CartDetail>();
    }
}

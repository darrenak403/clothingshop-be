using System.ComponentModel.DataAnnotations;

namespace ClothingShop.Domain.Entities
{
    public class Role : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        public ICollection<User> Users { get; set; }
    }
}

using ClothingShop.Domain.Entities;

namespace ClothingShop.Infrastructure.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<bool> IsSlugUnique(string slug);
    }
}

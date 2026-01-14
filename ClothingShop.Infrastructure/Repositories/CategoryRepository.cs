using ClothingShop.Domain.Entities;
using ClothingShop.Infrastructure.Interfaces;
using ClothingShop.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ClothingShop.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ClothingShopDbContext context) : base(context)
        {
        }

        public async Task<bool> IsSlugUnique(string slug, Guid? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return !await _dbSet.AnyAsync(p => p.Slug == slug && p.Id != excludeId.Value);
            }
            return !await _dbSet.AnyAsync(p => p.Slug == slug);
        }
    }
}

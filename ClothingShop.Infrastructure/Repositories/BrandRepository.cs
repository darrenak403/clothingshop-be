using ClothingShop.Domain.Entities;
using ClothingShop.Infrastructure.Interfaces;
using ClothingShop.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ClothingShop.Infrastructure.Repositories
{
    public class BrandRepository : GenericRepository<Brand>, IBrandRepository
    {
        public BrandRepository(ClothingShopDbContext context) : base(context)
        {
        }

        public async Task<bool> IsSlugUnique(string slug)
        {
            return !await _dbSet.AnyAsync(c => c.Slug == slug);
        }
    }
}

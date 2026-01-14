using ClothingShop.Domain.Entities;
using ClothingShop.Infrastructure.Interfaces;
using ClothingShop.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ClothingShop.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ClothingShopDbContext context) : base(context)
        {
        }

        //OVERRIDE để Include relationships
        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.Category)      // Load Category
                .Include(p => p.Brand)          // Load Brand (nullable)
                .Include(p => p.Variants)       // Load Variants (empty nếu chưa có)
                .Include(p => p.Images)         // Load Images (empty nếu chưa có)
                                                // .Include(p => p.Reviews)     // Không load Reviews ở đây (quá nhiều)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<bool> IsSlugUnique(string slug, Guid? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return !await _dbSet.AnyAsync(p => p.Slug == slug && p.Id != excludeId.Value);
            }
            return !await _dbSet.AnyAsync(p => p.Slug == slug);
        }

        public async Task<Product?> GetBySlugAsync(string slug)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Variants)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Slug == slug && !p.IsDeleted);
        }

        public async Task<IEnumerable<Product>> GetFeaturedAsync()
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.IsFeatured && p.IsActive && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.CategoryId == categoryId && p.IsActive && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByBrandAsync(Guid brandId)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.BrandId == brandId && p.IsActive && !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
}

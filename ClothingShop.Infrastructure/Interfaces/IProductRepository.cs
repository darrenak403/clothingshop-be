using ClothingShop.Domain.Entities;

namespace ClothingShop.Infrastructure.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        // Slug validation (giống Brand)
        Task<bool> IsSlugUnique(string slug, Guid? excludeId = null);

        //Custom methods cho Product
        Task<Product?> GetBySlugAsync(string slug);
        Task<IEnumerable<Product>> GetFeaturedAsync();
        Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId);
        Task<IEnumerable<Product>> GetByBrandAsync(Guid brandId);
    }
}

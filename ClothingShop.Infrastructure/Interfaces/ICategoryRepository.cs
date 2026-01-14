using ClothingShop.Domain.Entities;

namespace ClothingShop.Infrastructure.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        /// <summary>
        /// Kiểm tra slug có trùng không (dùng khi Create/Update)
        /// </summary>
        /// <param name="slug">Slug cần check</param>
        Task<bool> IsSlugUnique(string slug, Guid? excludeId = null);
    }
}

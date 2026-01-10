using ClothingShop.Domain.Entities;

namespace ClothingShop.Infrastructure.Interfaces
{
    public interface IBrandRepository : IGenericRepository<Brand>
    {
        /// <summary>
        /// Kiểm tra slug có trùng không (dùng khi Create/Update)
        /// </summary>
        /// <param name="slug">Slug cần check</param>
        /// <param name="excludeId">ID brand đang update (để loại trừ chính nó)</param>
        Task<bool> IsSlugUnique(string slug);
    }
}

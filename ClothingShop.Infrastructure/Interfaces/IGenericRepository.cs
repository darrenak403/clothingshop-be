using System.Linq.Expressions;

namespace ClothingShop.Infrastructure.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindListAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);

        // ⭐ THÊM: Update methods
        void Update(T entity);
        Task UpdateAsync(T entity);

        void Delete(T entity);

        // ⭐ THÊM: Soft delete
        void SoftDelete(T entity);
    }
}

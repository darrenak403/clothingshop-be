using ClothingShop.Domain.Entities;
using ClothingShop.Infrastructure.Interfaces;
using ClothingShop.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ClothingShop.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly ClothingShopDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ClothingShopDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // ⭐ FIX: Lọc IsDeleted
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.Where(e => !e.IsDeleted).ToListAsync();
        }

        // ⭐ FIX: GetById cũng phải lọc IsDeleted
        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> FindListAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        // ⭐ THÊM: Update method
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask; // Để giữ signature async
        }

        // Hard delete
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        // ⭐ THÊM: Soft delete method
        public void SoftDelete(T entity)
        {
            entity.IsDeleted = true;
            _dbSet.Update(entity);
        }
    }
}

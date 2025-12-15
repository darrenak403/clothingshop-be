using ClothingShop.Domain.Interfaces;
using ClothingShop.Infrastructure.Persistence.Context;
namespace ClothingShop.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly ClothingShopDbContext _context;
        public UnitOfWork(ClothingShopDbContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

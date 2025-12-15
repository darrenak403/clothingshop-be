namespace ClothingShop.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Khai báo các Repo cụ thể
        // IProductRepository Products { get; }
        // IOrderRepository Orders { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

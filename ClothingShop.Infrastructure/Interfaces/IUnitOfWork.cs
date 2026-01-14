namespace ClothingShop.Infrastructure.Interfaces
{
    /// <summary>
    /// Unit of Work Pattern - Quản lý tất cả repositories và đảm bảo tính nhất quán của giao dịch
    /// Thay vì inject từng repository riêng lẻ, chỉ cần inject IUnitOfWork
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Repository cho các entity chính
        IUserRepository Users { get; }
        IAddressRepository Addresses { get; }
        IRoleRepository Roles { get; }
        IPasswordResetHistoryRepository PasswordResets { get; }
        ICategoryRepository Categories { get; }
        IBrandRepository Brands { get; }
        IProductRepository Products { get; }

        // Có thể thêm các repository khác khi cần
        // IGenericRepository<Product> Products { get; }
        // IGenericRepository<Order> Orders { get; }
        // IGenericRepository<Cart> Carts { get; }

        /// <summary>
        /// Lưu tất cả thay đổi vào database trong một transaction duy nhất
        /// Đảm bảo "Tất cả cùng thành công" hoặc "Tất cả cùng rollback"
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

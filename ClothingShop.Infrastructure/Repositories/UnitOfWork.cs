using ClothingShop.Domain.Entities;
using ClothingShop.Domain.Interfaces;
using ClothingShop.Infrastructure.Persistence.Context;

namespace ClothingShop.Infrastructure.Repositories
{
    /// <summary>
    /// Implementation của Unit of Work Pattern
    /// Tạo và quản lý các repository instances, chia sẻ chung 1 DbContext
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ClothingShopDbContext _context;

        // Lazy initialization - chỉ tạo repository khi cần dùng
        private IGenericRepository<User>? _users;
        private IRoleRepository? _roles;
        private IPasswordResetHistoryRepository? _passwordResets;

        public UnitOfWork(ClothingShopDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Repository cho User - Lazy initialization
        /// Chỉ tạo instance khi được gọi lần đầu tiên
        /// </summary>
        public IGenericRepository<User> Users
        {
            get
            {
                _users ??= new GenericRepository<User>(_context);
                return _users;
            }
        }

        /// <summary>
        /// Repository cho Role - Lazy initialization
        /// </summary>
        public IRoleRepository Roles
        {
            get
            {
                _roles ??= new RoleRepository(_context);
                return _roles;
            }
        }

        /// <summary>
        /// Repository cho PasswordResetHistory - Lazy initialization
        /// </summary>
        public IPasswordResetHistoryRepository PasswordResets
        {
            get
            {
                _passwordResets ??= new PasswordResetHistoryRepository(_context);
                return _passwordResets;
            }
        }

        /// <summary>
        /// Lưu tất cả thay đổi vào database
        /// Tất cả operations từ các repositories sẽ được commit cùng lúc
        /// Nếu có lỗi, tất cả sẽ được rollback (Transaction)
        /// </summary>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Giải phóng DbContext khi không dùng nữa
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

using ClothingShop.Infrastructure.Interfaces;
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

        private IRoleRepository? _roles;
        private IUserRepository? _users;
        private IAddressRepository? _addresses;
        private IPasswordResetHistoryRepository? _passwordResets;

        public UnitOfWork(ClothingShopDbContext context)
        {
            _context = context;
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

        public IUserRepository Users
        {
            get
            {
                _users ??= new UserRepository(_context);
                return _users;
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

        public IAddressRepository Addresses
        {
            get
            {
                _addresses ??= new AddressRepository(_context);
                return _addresses;
            }
        }

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

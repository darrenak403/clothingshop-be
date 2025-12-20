using ClothingShop.Domain.Entities;

namespace ClothingShop.Infrastructure.Interfaces
{
    public interface IAddressRepository : IGenericRepository<Address>
    {
        Task<IEnumerable<Address>> GetByUserIdAsync(Guid userId);
        // Hàm reset tất cả địa chỉ default của 1 user về false
        Task ResetDefaultAddressAsync(Guid userId);
    }
}

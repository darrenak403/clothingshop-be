using ClothingShop.Domain.Entities;

namespace ClothingShop.Infrastructure.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<(IEnumerable<User> Items, int TotalCount)> GetPagedUsersAsync(string? keyword, bool? isActive, int pageNumber, int pageSize);
    }
}

using ClothingShop.Domain.Entities;

namespace ClothingShop.Domain.Interfaces
{
    public interface IPasswordResetHistoryRepository
    {
        Task AddAsync(PasswordResetHistory entity);
        Task UpdateAsync(PasswordResetHistory entity);
        Task<IEnumerable<PasswordResetHistory>> GetAllUsedOtpAsync(System.Linq.Expressions.Expression<Func<PasswordResetHistory, bool>> predicate);

        Task<PasswordResetHistory?> GetLatestValidOtpAsync(Guid userId, string otp); // int → Guid
        Task<int> GetTodayRequestCountAsync(Guid userId); // int → Guid
        Task<List<PasswordResetHistory>> GetUserHistoryAsync(Guid userId, int pageSize = 50); // int → Guid
        Task MarkAllAsExpiredAsync(Guid userId); // int → Guid
        Task<PasswordResetHistory?> GetByIdAsync(Guid id);
    }
}

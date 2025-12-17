using ClothingShop.Domain.Entities;

namespace ClothingShop.Domain.Interfaces
{
    public interface IPasswordResetHistoryRepository
    {
        Task<PasswordResetHistory?> GetLatestValidOtpAsync(Guid userId, string otp);
        Task<int> GetTodayRequestCountAsync(Guid userId);
        Task<List<PasswordResetHistory>> GetUserHistoryAsync(Guid userId, int pageSize = 50);
        Task MarkAllAsExpiredAsync(Guid userId);
    }
}

using ClothingShop.Domain.Entities;
using ClothingShop.Domain.Enums;
using ClothingShop.Domain.Interfaces;
using ClothingShop.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ClothingShop.Infrastructure.Repositories
{
    public class PasswordResetHistoryRepository : IPasswordResetHistoryRepository
    {
        protected readonly ClothingShopDbContext _context;

        public PasswordResetHistoryRepository(ClothingShopDbContext context)
        {
            _context = context;
        }

        public async Task<PasswordResetHistory?> GetLatestValidOtpAsync(Guid userId, string otp)
        {
            return await _context.PasswordResetHistories
                .Where(p => p.UserId == userId
                    && p.Otp == otp
                    && !p.IsUsed
                    && p.IsExpried > DateTime.UtcNow
                    && p.Status == AttemptStatus.Pending)
                .OrderByDescending(p => p.OtpGeneratedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetTodayRequestCountAsync(Guid userId)
        {
            var today = DateTime.UtcNow.Date;
            return await _context.PasswordResetHistories
                .Where(p => p.UserId == userId && p.OtpGeneratedAt >= today)
                .CountAsync();
        }

        public async Task<List<PasswordResetHistory>> GetUserHistoryAsync(Guid userId, int pageSize = 50)
        {
            return await _context.PasswordResetHistories
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.OtpGeneratedAt)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task MarkAllAsExpiredAsync(Guid userId)
        {
            var pendingOtps = await _context.PasswordResetHistories
                .Where(p => p.UserId == userId && p.Status == AttemptStatus.Pending && !p.IsUsed)
                .ToListAsync();

            foreach (var otp in pendingOtps)
            {
                otp.Status = AttemptStatus.Expired;
            }

            await _context.SaveChangesAsync();
        }
    }
}
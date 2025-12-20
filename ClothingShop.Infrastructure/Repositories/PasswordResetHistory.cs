using ClothingShop.Domain.Entities;
using ClothingShop.Domain.Enums;
using ClothingShop.Infrastructure.Interfaces;
using ClothingShop.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ClothingShop.Infrastructure.Repositories
{
    public class PasswordResetHistoryRepository : IPasswordResetHistoryRepository
    {
        private readonly ClothingShopDbContext _context;

        public PasswordResetHistoryRepository(ClothingShopDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PasswordResetHistory entity) => await _context.PasswordResetHistories.AddAsync(entity);

        public async Task UpdateAsync(PasswordResetHistory entity) => _context.PasswordResetHistories.Update(entity);

        public async Task<IEnumerable<PasswordResetHistory>> GetAllAsync() => await _context.PasswordResetHistories.ToListAsync();

        public async Task<IEnumerable<PasswordResetHistory>> GetAllUsedOtpAsync(System.Linq.Expressions.Expression<Func<PasswordResetHistory, bool>> predicate)
        {
            return await _context.PasswordResetHistories.Where(predicate).ToListAsync();
        }

        public async Task<PasswordResetHistory?> GetLatestValidOtpAsync(Guid userId, string otp)
        {
            return await _context.PasswordResetHistories
                .Where(p => p.UserId == userId
                    && p.Otp == otp
                    && !p.IsUsed
                    && p.OtpExpiresAt > DateTime.UtcNow
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

        public async Task<PasswordResetHistory?> GetByIdAsync(Guid id)
        {
            return await _context.PasswordResetHistories.FindAsync(id);
        }
    }
}
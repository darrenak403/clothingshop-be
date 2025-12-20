using ClothingShop.Domain.Entities;
using ClothingShop.Infrastructure.Interfaces;
using ClothingShop.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ClothingShop.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ClothingShopDbContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<User> Items, int TotalCount)> GetPagedUsersAsync(string? keyword, bool? isActive, int pageNumber, int pageSize)
        {
            // 1. Khởi tạo Queryable (chưa chạy xuống DB)
            var query = _context.Users
                 .Include(u => u.Role)
                 .Include(u => u.Orders)
                 .AsQueryable();

            // 2. Lọc theo từ khóa (Tên hoặc Email)
            if (!string.IsNullOrEmpty(keyword))
            {
                var lowerKeyword = keyword.ToLower();
                query = query.Where(u => u.FullName.ToLower().Contains(lowerKeyword)
                                      || u.Email.ToLower().Contains(lowerKeyword));
            }

            // 3. Lọc theo trạng thái (Active/Locked)
            if (isActive.HasValue)
            {
                query = query.Where(u => u.IsActive == isActive.Value);
            }

            // 4. Đếm tổng số bản ghi TRƯỚC khi phân trang (để tính TotalPages)
            var totalCount = await query.CountAsync();

            // 5. Phân trang và sắp xếp
            var items = await query
                .OrderByDescending(u => u.CreatedAt) // Người mới nhất lên đầu
                .Skip((pageNumber - 1) * pageSize)   // Bỏ qua các trang trước
                .Take(pageSize)                      // Lấy số lượng trang hiện tại
                .ToListAsync();                      // Thực thi query

            return (items, totalCount);
        }
    }
}

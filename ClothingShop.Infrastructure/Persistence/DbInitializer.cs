using ClothingShop.Domain.Entities;
using ClothingShop.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ClothingShop.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static void Initialize(ClothingShopDbContext context)
        {
            // 1. Đảm bảo Database đã được tạo
            context.Database.Migrate();

            // 2. Check xem đã có User Admin nào chưa
            // (Dựa vào RoleId = 1 là Admin, hoặc Email)
            if (context.Users.Any(u => u.Email == "admin@admin.com"))
            {
                return; // Đã có rồi thì thôi, không làm gì cả
            }

            // 3. Nếu chưa có, tạo mới User Admin
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Administrator",
                Email = "admin@admin.com",
                PhoneNumber = "0799995828",
                // Mật khẩu mặc định là: Admin@123
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin@123"),
                RoleId = 1, // Role ID 1 là Admin (như đã seed trong Context)
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            context.Users.Add(adminUser);
            context.SaveChanges();
        }
    }
}
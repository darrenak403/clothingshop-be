using ClothingShop.Domain.Entities;
using ClothingShop.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ClothingShop.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static void Initialize(ClothingShopDbContext context)
        {
            // 1. Đảm bảo Database đã được tạo và update mới nhất
            context.Database.Migrate();

            // ==================================================
            // 2. SEED ROLES (Chuyển từ DbContext sang đây)
            // ==================================================
            if (!context.Roles.Any()) // Nếu bảng Role trống trơn
            {
                var roles = new List<Role>
                {
                    new Role
                    {
                        Id = 1,
                        Name = "Admin",
                        CreatedAt = DateTime.UtcNow, // <--- ĐÃ OK: Lấy giờ hiện tại khi chạy app
                        IsDeleted = false
                    },
                    new Role
                    {
                        Id = 2,
                        Name = "Staff",
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    },
                    new Role
                    {
                        Id = 3,
                        Name = "Customer",
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    }
                };

                context.Roles.AddRange(roles);
                context.SaveChanges(); // Lưu Role trước để có ID (nếu dùng Identity tự tăng)
            }

            // ==================================================
            // 3. SEED ADMIN USER
            // ==================================================
            if (!context.Users.Any(u => u.Email == "admin@admin.com"))
            {
                // Tìm ID của Role Admin vừa tạo ở trên (đề phòng ID không phải là 1)
                // Lưu ý: Nếu bạn set cứng ID cho Role thì có thể gán thẳng = 1
                var adminRole = context.Roles.First(r => r.Name == "Admin");

                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "Administrator",
                    Email = "admin@admin.com",
                    PhoneNumber = "0799995828",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin@123"),

                    // Gán Role
                    Role = adminRole, // Gán object Role thay vì ID để an toàn hơn
                                      // Hoặc: RoleId = adminRole.Id,

                    CreatedAt = DateTime.UtcNow, // Giờ động
                    IsDeleted = false
                };

                context.Users.Add(adminUser);
                context.SaveChanges();
            }
        }
    }
}
using ClothingShop.Domain.Entities;
using ClothingShop.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClothingShop.Infrastructure.Persistence.Context
{
    public class ClothingShopDbContext : DbContext
    {
        public ClothingShopDbContext(DbContextOptions<ClothingShopDbContext> options) : base(options)
        {
        }

        // =========================================================
        // 1. KHAI BÁO CÁC BẢNG (DBSETS)
        // =========================================================

        // --- Nhóm Identity (Người dùng) ---
        public DbSet<User> Users { get; set; }
        public DbSet<PasswordResetHistory> PasswordResetHistories { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Address> Addresses { get; set; }

        // --- Nhóm Catalog (Sản phẩm & Kho) ---
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        // --- Nhóm Sales (Đơn hàng & Thanh toán) ---
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }

        // --- Nhóm Marketing & Tiện ích ---
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<Review> Reviews { get; set; }


        // =========================================================
        // 2. CẤU HÌNH FLUENT API (ON MODEL CREATING)
        // =========================================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --------------------------------------------------
            // A. Cấu hình Global (Tự động áp dụng cho toàn bộ)
            // --------------------------------------------------

            // 1. Xử lý Decimal (Tiền tệ): Tránh lỗi làm tròn số trong MySQL/SQL Server
            // Tự động tìm tất cả property kiểu decimal và set precision (18, 2)
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?));

                foreach (var property in properties)
                {
                    modelBuilder.Entity(entityType.Name).Property(property.Name).HasPrecision(18, 2);
                }
            }

            // --------------------------------------------------
            // B. Cấu hình Chi tiết từng Entity
            // --------------------------------------------------

            // --- USER ---
            modelBuilder.Entity<User>(e =>
            {
                e.HasIndex(u => u.Email).IsUnique(); // Email là duy nhất
                e.Property(u => u.Email).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<PasswordResetHistory>(entity =>
            {
                entity.ToTable("PasswordResetHistories");

                entity.HasKey(e => e.UserId);

                entity.Property(e => e.Otp)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue(AttemptStatus.Pending);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.PasswordResetHistories)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => new { e.UserId, e.OtpGeneratedAt });
            });

            // --- CATEGORY ---
            modelBuilder.Entity<Category>(e =>
            {
                e.HasIndex(c => c.Slug).IsUnique(); // Slug (URL) không trùng

                // Quan hệ đệ quy (Cha - Con): Xóa cha KHÔNG được xóa con (Restrict)
                e.HasOne(c => c.Parent)
                 .WithMany(c => c.SubCategories)
                 .HasForeignKey(c => c.ParentId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // --- BRAND & PRODUCT ---
            modelBuilder.Entity<Brand>(e => { e.HasIndex(b => b.Slug).IsUnique(); });
            modelBuilder.Entity<Product>(e => { e.HasIndex(p => p.Slug).IsUnique(); });

            // --- PRODUCT VARIANT (QUAN TRỌNG) ---
            modelBuilder.Entity<ProductVariant>(e =>
            {
                e.HasIndex(v => v.Sku).IsUnique(); // Mã SKU (Mã vạch) phải duy nhất toàn hệ thống

                // Composite Unique Index: Một sản phẩm không thể có 2 dòng trùng Size và Color
                e.HasIndex(v => new { v.ProductId, v.Size, v.Color }).IsUnique();
            });

            // --- ORDER ---
            modelBuilder.Entity<Order>(e =>
            {
                // Lưu Enum dạng chuỗi (String) để dễ đọc trong Database (vd: "Confirmed" thay vì số 1)
                e.Property(o => o.Status).HasConversion<string>();
                e.Property(o => o.PaymentMethod).HasConversion<string>();
                e.Property(o => o.PaymentStatus).HasConversion<string>();
            });

            // --- ORDER ITEM ---
            modelBuilder.Entity<OrderDetail>(e =>
            {
                // BẢO VỆ DỮ LIỆU: Nếu xóa sản phẩm trong kho (ProductVariant), 
                // thì KHÔNG ĐƯỢC xóa dòng này trong lịch sử đơn hàng.
                e.HasOne(d => d.ProductVariant)
                 .WithMany()
                 .HasForeignKey(d => d.ProductVariantId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // --- VOUCHER ---
            modelBuilder.Entity<Voucher>(e => { e.HasIndex(v => v.Code).IsUnique(); });
        }
    }
}
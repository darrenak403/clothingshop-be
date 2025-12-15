using ClothingShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClothingShop.Infrastructure.Persistence.Context
{
    public class ClothingShopDbContext : DbContext
    {
        public ClothingShopDbContext(DbContextOptions<ClothingShopDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        // Các DbSet khác...

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Data cho Role (Admin, Staff, Customer)
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Staff" },
                new Role { Id = 3, Name = "Customer" }
            );
        }
    }
}

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
            // 2. SEED ROLES
            // ==================================================
            if (!context.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new Role { Id = 1, Name = "Admin", CreatedAt = DateTime.UtcNow, IsDeleted = false },
                    new Role { Id = 2, Name = "Staff", CreatedAt = DateTime.UtcNow, IsDeleted = false },
                    new Role { Id = 3, Name = "Customer", CreatedAt = DateTime.UtcNow, IsDeleted = false }
                };
                context.Roles.AddRange(roles);
                context.SaveChanges();
            }

            // ==================================================
            // 3. SEED ADMIN USER
            // ==================================================
            if (!context.Users.Any(u => u.Email == "admin@admin.com"))
            {
                var adminRole = context.Roles.First(r => r.Name == "Admin");
                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "Administrator",
                    Email = "admin@admin.com",
                    PhoneNumber = "0799995828",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin@123"),
                    Role = adminRole,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };
                context.Users.Add(adminUser);
                context.SaveChanges();
            }

            // ==================================================
            // 4. SEED BRANDS (Thương hiệu)
            // ==================================================
            if (!context.Brands.Any())
            {
                var brands = new List<Brand>
                {
                    new Brand
                    {
                        Id = Guid.NewGuid(),
                        Name = "Nike",
                        Slug = "nike",
                        LogoUrl = "https://res.cloudinary.com/demo/image/upload/v1234567890/brands/nike-logo.png",
                        Website = "https://www.nike.com",
                        Description = "Just Do It - Thương hiệu thể thao hàng đầu thế giới",
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    },
                    new Brand
                    {
                        Id = Guid.NewGuid(),
                        Name = "Adidas",
                        Slug = "adidas",
                        LogoUrl = "https://res.cloudinary.com/demo/image/upload/v1234567890/brands/adidas-logo.png",
                        Website = "https://www.adidas.com",
                        Description = "Impossible is Nothing - Thương hiệu thể thao Đức",
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    },
                    new Brand
                    {
                        Id = Guid.NewGuid(),
                        Name = "Uniqlo",
                        Slug = "uniqlo",
                        LogoUrl = "https://res.cloudinary.com/demo/image/upload/v1234567890/brands/uniqlo-logo.png",
                        Website = "https://www.uniqlo.com",
                        Description = "LifeWear - Thời trang Nhật Bản tiện dụng",
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    },
                    new Brand
                    {
                        Id = Guid.NewGuid(),
                        Name = "Zara",
                        Slug = "zara",
                        LogoUrl = "https://res.cloudinary.com/demo/image/upload/v1234567890/brands/zara-logo.png",
                        Website = "https://www.zara.com",
                        Description = "Thời trang cao cấp từ Tây Ban Nha",
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    },
                    new Brand
                    {
                        Id = Guid.NewGuid(),
                        Name = "H&M",
                        Slug = "hm",
                        LogoUrl = "https://res.cloudinary.com/demo/image/upload/v1234567890/brands/hm-logo.png",
                        Website = "https://www.hm.com",
                        Description = "Thời trang bền vững từ Thụy Điển",
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    }
                };
                context.Brands.AddRange(brands);
                context.SaveChanges();
            }

            // ==================================================
            // 5. SEED CATEGORIES (Danh mục cây)
            // ==================================================
            if (!context.Categories.Any())
            {
                // Level 1: Root categories
                var catNam = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Thời trang Nam",
                    Slug = "thoi-trang-nam",
                    Description = "Tất cả sản phẩm dành cho Nam",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                var catNu = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Thời trang Nữ",
                    Slug = "thoi-trang-nu",
                    Description = "Tất cả sản phẩm dành cho Nữ",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                var catTreEm = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Thời trang Trẻ em",
                    Slug = "thoi-trang-tre-em",
                    Description = "Sản phẩm dành cho trẻ em",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                context.Categories.AddRange(catNam, catNu, catTreEm);
                context.SaveChanges(); // Save để có Id

                // Level 2: Sub categories cho Nam
                var subCatNam = new List<Category>
                {
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Áo thun Nam",
                        Slug = "ao-thun-nam",
                        Description = "Áo thun cotton, polo nam",
                        ParentId = catNam.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    },
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Áo sơ mi Nam",
                        Slug = "ao-so-mi-nam",
                        Description = "Áo sơ mi công sở, casual",
                        ParentId = catNam.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    },
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Quần Jean Nam",
                        Slug = "quan-jean-nam",
                        Description = "Quần jean nam các kiểu dáng",
                        ParentId = catNam.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    },
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Quần short Nam",
                        Slug = "quan-short-nam",
                        Description = "Quần short thể thao, casual",
                        ParentId = catNam.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    }
                };

                // Level 2: Sub categories cho Nữ
                var subCatNu = new List<Category>
                {
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Áo kiểu Nữ",
                        Slug = "ao-kieu-nu",
                        Description = "Áo kiểu, áo sơ mi nữ",
                        ParentId = catNu.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    },
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Váy Nữ",
                        Slug = "vay-nu",
                        Description = "Váy dài, váy ngắn, đầm",
                        ParentId = catNu.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    },
                    new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Quần Jean Nữ",
                        Slug = "quan-jean-nu",
                        Description = "Quần jean nữ skinny, baggy",
                        ParentId = catNu.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    }
                };

                context.Categories.AddRange(subCatNam);
                context.Categories.AddRange(subCatNu);
                context.SaveChanges();
            }

            // ==================================================
            // 6. SEED PRODUCTS (Sản phẩm)
            // ==================================================
            if (!context.Products.Any())
            {
                // Lấy Brand và Category IDs
                var nikeBrand = context.Brands.First(b => b.Slug == "nike");
                var adidasBrand = context.Brands.First(b => b.Slug == "adidas");
                var uniqloBrand = context.Brands.First(b => b.Slug == "uniqlo");

                var aoThunNamCat = context.Categories.First(c => c.Slug == "ao-thun-nam");
                var aoSoMiNamCat = context.Categories.First(c => c.Slug == "ao-so-mi-nam");
                var quanJeanNamCat = context.Categories.First(c => c.Slug == "quan-jean-nam");
                var aoKieuNuCat = context.Categories.First(c => c.Slug == "ao-kieu-nu");
                var vayNuCat = context.Categories.First(c => c.Slug == "vay-nu");

                var products = new List<Product>
                {
                    // Sản phẩm Nike
                    new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "Áo thun Nike Dri-FIT",
                        Slug = "ao-thun-nike-dri-fit",
                        Description = "Áo thun thể thao Nike công nghệ Dri-FIT thấm hút mồ hôi",
                        Content = "<p>Áo thun Nike Dri-FIT với công nghệ thấm hút mồ hôi vượt trội, chất liệu cotton thoáng mát, phù hợp tập gym và chơi thể thao.</p>",
                        Price = 599000,
                        OriginalPrice = 899000,
                        Thumbnail = "https://static.nike.com/a/images/t_PDP_1728_v1/f_auto,q_auto:eco/abc123/nike-dri-fit-tee.png",
                        CategoryId = aoThunNamCat.Id,
                        BrandId = nikeBrand.Id,
                        IsFeatured = true,
                        IsActive = true,
                        ViewCount = 1250,
                        SoldCount = 180,
                        MetaTitle = "Áo thun Nike Dri-FIT | Thấm hút mồ hôi",
                        MetaKeyword = "áo thun nike, dri-fit, thể thao",
                        MetaDescription = "Áo thun Nike Dri-FIT chính hãng giá tốt",
                        CreatedAt = DateTime.UtcNow.AddDays(-15),
                        IsDeleted = false
                    },
                    new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "Áo polo Nike Sportswear",
                        Slug = "ao-polo-nike-sportswear",
                        Description = "Áo polo Nike cao cấp, thiết kế hiện đại",
                        Content = "<p>Áo polo Nike Sportswear với chất liệu cotton pha cao cấp, form dáng slimfit trẻ trung.</p>",
                        Price = 799000,
                        OriginalPrice = 1200000,
                        Thumbnail = "https://static.nike.com/a/images/t_PDP_1728_v1/f_auto,q_auto:eco/def456/nike-polo.png",
                        CategoryId = aoThunNamCat.Id,
                        BrandId = nikeBrand.Id,
                        IsFeatured = true,
                        IsActive = true,
                        ViewCount = 890,
                        SoldCount = 95,
                        MetaTitle = "Áo polo Nike Sportswear",
                        MetaKeyword = "áo polo nike, sportswear",
                        CreatedAt = DateTime.UtcNow.AddDays(-10),
                        IsDeleted = false
                    },

                    // Sản phẩm Adidas
                    new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "Áo thun Adidas Essentials",
                        Slug = "ao-thun-adidas-essentials",
                        Description = "Áo thun Adidas basic cotton 100%",
                        Content = "<p>Áo thun Adidas Essentials với chất liệu cotton 100%, logo 3 sọc đặc trưng.</p>",
                        Price = 450000,
                        OriginalPrice = 650000,
                        Thumbnail = "https://assets.adidas.com/images/t_PDP_1728_v1/f_auto,q_auto:eco/ghi789/adidas-essentials-tee.jpg",
                        CategoryId = aoThunNamCat.Id,
                        BrandId = adidasBrand.Id,
                        IsFeatured = false,
                        IsActive = true,
                        ViewCount = 650,
                        SoldCount = 120,
                        MetaTitle = "Áo thun Adidas Essentials",
                        MetaKeyword = "áo thun adidas, essentials",
                        CreatedAt = DateTime.UtcNow.AddDays(-8),
                        IsDeleted = false
                    },

                    // Sản phẩm Uniqlo
                    new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "Áo sơ mi Uniqlo Oxford",
                        Slug = "ao-so-mi-uniqlo-oxford",
                        Description = "Áo sơ mi Uniqlo vải Oxford cao cấp",
                        Content = "<p>Áo sơ mi Uniqlo Oxford với chất liệu vải Oxford dày dặn, bền đẹp, phù hợp công sở.</p>",
                        Price = 590000,
                        OriginalPrice = 790000,
                        Thumbnail = "https://image.uniqlo.com/UQ/ST3/WesternCommon/imagesgoods/jkl012/item/goods_69_jkl012.jpg",
                        CategoryId = aoSoMiNamCat.Id,
                        BrandId = uniqloBrand.Id,
                        IsFeatured = true,
                        IsActive = true,
                        ViewCount = 1100,
                        SoldCount = 210,
                        MetaTitle = "Áo sơ mi Uniqlo Oxford",
                        MetaKeyword = "áo sơ mi uniqlo, oxford",
                        CreatedAt = DateTime.UtcNow.AddDays(-12),
                        IsDeleted = false
                    },
                    new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "Quần jean Uniqlo Selvedge",
                        Slug = "quan-jean-uniqlo-selvedge",
                        Description = "Quần jean Uniqlo dáng regular fit",
                        Content = "<p>Quần jean Uniqlo Selvedge với dáng regular fit thoải mái, chất denim Nhật cao cấp.</p>",
                        Price = 990000,
                        OriginalPrice = 1290000,
                        Thumbnail = "https://image.uniqlo.com/UQ/ST3/WesternCommon/imagesgoods/mno345/item/goods_69_mno345.jpg",
                        CategoryId = quanJeanNamCat.Id,
                        BrandId = uniqloBrand.Id,
                        IsFeatured = false,
                        IsActive = true,
                        ViewCount = 780,
                        SoldCount = 65,
                        MetaTitle = "Quần jean Uniqlo Selvedge",
                        MetaKeyword = "quần jean uniqlo, selvedge",
                        CreatedAt = DateTime.UtcNow.AddDays(-5),
                        IsDeleted = false
                    },

                    // Sản phẩm Nữ
                    new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "Áo kiểu nữ Uniqlo lụa",
                        Slug = "ao-kieu-nu-uniqlo-lua",
                        Description = "Áo kiểu nữ Uniqlo vải lụa mềm mại",
                        Content = "<p>Áo kiểu nữ Uniqlo chất liệu lụa mềm mại, thiết kế thanh lịch phù hợp công sở.</p>",
                        Price = 690000,
                        OriginalPrice = 990000,
                        Thumbnail = "https://image.uniqlo.com/UQ/ST3/WesternCommon/imagesgoods/pqr678/item/goods_01_pqr678.jpg",
                        CategoryId = aoKieuNuCat.Id,
                        BrandId = uniqloBrand.Id,
                        IsFeatured = true,
                        IsActive = true,
                        ViewCount = 920,
                        SoldCount = 145,
                        MetaTitle = "Áo kiểu nữ Uniqlo lụa",
                        MetaKeyword = "áo kiểu nữ uniqlo, lụa",
                        CreatedAt = DateTime.UtcNow.AddDays(-7),
                        IsDeleted = false
                    },
                    new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "Váy midi Uniqlo thanh lịch",
                        Slug = "vay-midi-uniqlo-thanh-lich",
                        Description = "Váy midi Uniqlo dáng chữ A thanh lịch",
                        Content = "<p>Váy midi Uniqlo dáng chữ A với chất liệu vải mềm mại, thiết kế thanh lịch.</p>",
                        Price = 890000,
                        OriginalPrice = 1190000,
                        Thumbnail = "https://image.uniqlo.com/UQ/ST3/WesternCommon/imagesgoods/stu901/item/goods_01_stu901.jpg",
                        CategoryId = vayNuCat.Id,
                        BrandId = uniqloBrand.Id,
                        IsFeatured = false,
                        IsActive = true,
                        ViewCount = 560,
                        SoldCount = 72,
                        MetaTitle = "Váy midi Uniqlo thanh lịch",
                        MetaKeyword = "váy midi uniqlo",
                        CreatedAt = DateTime.UtcNow.AddDays(-3),
                        IsDeleted = false
                    },

                    // Sản phẩm không brand (test BrandId nullable)
                    new Product
                    {
                        Id = Guid.NewGuid(),
                        Name = "Áo thun trơn basic",
                        Slug = "ao-thun-tron-basic",
                        Description = "Áo thun trơn cotton 100% giá rẻ",
                        Content = "<p>Áo thun trơn basic với chất liệu cotton 100%, form rộng thoải mái.</p>",
                        Price = 150000,
                        OriginalPrice = 250000,
                        Thumbnail = "https://via.placeholder.com/500x500/333333/FFFFFF?text=Basic+Tee",
                        CategoryId = aoThunNamCat.Id,
                        BrandId = null, // ⭐ Không thuộc brand nào
                        IsFeatured = false,
                        IsActive = true,
                        ViewCount = 320,
                        SoldCount = 85,
                        MetaTitle = "Áo thun trơn basic",
                        MetaKeyword = "áo thun trơn, basic",
                        CreatedAt = DateTime.UtcNow.AddDays(-2),
                        IsDeleted = false
                    }
                };

                context.Products.AddRange(products);
                context.SaveChanges();
            }
        }
    }
}
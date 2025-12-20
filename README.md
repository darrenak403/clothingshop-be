# ClothingShop - E-commerce Platform

> **Dự án:** Hệ thống bán quần áo E-commerce  
> **Kiến trúc:** Clean Architecture (Onion Architecture)  
> **Database:** MySQL  
> **Backend:** ASP.NET Core 8 Web API

---

## 🏗️ 1. Nguyên tắc cốt lõi (Core Principles)

1.  **Dependency Rule (Quy tắc phụ thuộc):** Các tầng bên trong **KHÔNG BAO GIỜ** phụ thuộc vào các tầng bên ngoài.
    - `Domain` (Tâm) không biết ai cả.
    - `Application` chỉ biết `Domain`.
    - `Infrastructure` và `API` biết `Application` và `Domain`.
2.  **Code First:** Mọi thay đổi Database phải bắt đầu từ Code (Entity) -> Migration -> Database.
3.  **DTO First:** Controller không bao giờ trả về Entity trực tiếp. Luôn phải map sang DTO.
4.  **Repository Pattern:** Không truy vấn DB trực tiếp trong Controller hay Service. Phải qua Repository.

---

## 🗺️ 2. Cấu trúc dự án (Project Structure)

```plaintext
ClothingShop/
│
├── src/
│   ├── ClothingShop.API/ (Presentation Layer)
│   │   ├── Controllers/       # Chỉ nhận Request, gọi Service, trả Response
│   │   ├── Middleware/        # Xử lý Global (Error Handling, Logging)
│   │   └── Program.cs         # Entry point, DI Container
│   │
│   ├── ClothingShop.Application/ (Logic Layer)
│   │   ├── DTOs/              # Data Transfer Objects (Request/Response models)
│   │   ├── Interfaces/        # Contracts (IGenericRepository, IAuthService...)
│   │   ├── Services/          # Logic nghiệp vụ chính (ProductService, OrderService)
│   │   ├── Mappings/          # AutoMapper Profiles
│   │   └── Validators/        # FluentValidation
│   │
│   ├── ClothingShop.Domain/ (Core Layer - The Heart)
│   │   ├── Entities/          # Các bảng DB (User, Product, Order...)
│   │   ├── Common/            # BaseEntity (Id, CreatedAt, IsDeleted)
│   │   ├── Enums/             # Các định nghĩa cứng (UserRole, OrderStatus)
│   │   └── Specifications/    # Logic lọc query phức tạp
│   │
│   └── ClothingShop.Infrastructure/ (Data & External Layer)
│       ├── Persistence/       # DbContext, Migrations
│       ├── Repositories/      # Thực thi Interfaces Repository
│       └── Services/          # Các dịch vụ bên ngoài (Email, Payment, Upload File)
│
└── tests/                     # Unit Tests & Integration Tests
```

---

## 📏 3. Quy ước đặt tên (Naming Conventions)

| Thành phần         | Quy tắc               | Ví dụ                                 |
| :----------------- | :-------------------- | :------------------------------------ |
| **Interface**      | Bắt đầu bằng `I`      | `IProductService`, `IUnitOfWork`      |
| **Async Method**   | Kết thúc bằng `Async` | `GetByIdAsync`, `CreateUserAsync`     |
| **Class/Method**   | PascalCase            | `ProductController`, `CalculateTotal` |
| **Variable/Param** | camelCase             | `productId`, `currentUser`            |
| **Private Field**  | Bắt đầu bằng `_`      | `_context`, `_logger`                 |
| **Table DB**       | Số nhiều (Plural)     | `Products`, `Users`, `Orders`         |

---

## 🛠️ 4. Quy trình phát triển (Workflow)

### A. Thêm bảng mới hoặc sửa Database

1.  Vào `Domain/Entities`: Tạo/Sửa class Entity.
2.  Vào `Infrastructure/Data/AppDbContext.cs`: Khai báo `DbSet<>`.
3.  Mở Terminal chạy Migration:
    ```bash
    dotnet ef migrations add Ten_Migration -p src/ClothingShop.Infrastructure -s src/ClothingShop.API
    ```
4.  Update Database:
    ```bash
    dotnet ef database update -p src/ClothingShop.Infrastructure -s src/ClothingShop.API
    ```

### B. Tạo API mới (Ví dụ: Create Product)

1.  **Domain:** Định nghĩa Entity `Product`.
2.  **Infrastructure:** Viết Repository (nếu logic query phức tạp) hoặc dùng GenericRepo.
3.  **Application:**
    - Tạo `CreateProductDto`.
    - Tạo Validator (`CreateProductValidator`).
    - Viết Interface `IProductService`.
    - Implement `ProductService`.
4.  **API:** Tạo `ProductsController` -\> Inject Service -\> Gọi hàm.

---

## ⚠️ 5. Những điều cấm kỵ (Don'ts)

- ❌ **KHÔNG** đặt logic nghiệp vụ trong Controller. Controller càng mỏng càng tốt.
- ❌ **KHÔNG** dùng `DbContext` trực tiếp trong Controller.
- ❌ **KHÔNG** hard-code connection string hay secret key trong code (dùng `appsettings.json`).
- ❌ **KHÔNG** xóa dữ liệu thật (Hard Delete). Hãy dùng **Soft Delete** (`IsDeleted = true`).

---

## 🚀 6. Cheatsheet lệnh thường dùng

**Chạy dự án:**

```bash
dotnet run --project src/ClothingShop.API
```

**Sửa lỗi Migration (nếu lỡ tạo sai):**

```bash
dotnet ef migrations remove -p src/ClothingShop.Infrastructure -s src/ClothingShop.API
```

**Cập nhật thư viện:**

```bash
dotnet restore
```

```

### 👉 Việc tiếp theo của bạn:
Lưu file này lại. Bất cứ khi nào bạn quên lệnh Migration hoặc không nhớ nên viết code vào tầng nào, hãy mở file này ra xem!
```

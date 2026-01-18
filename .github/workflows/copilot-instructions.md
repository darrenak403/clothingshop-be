---
name: clothingshop-rules
description: ASP.NET Core Best Practices and Coding Standards for ClothingShop E-commerce Project
---

# ClothingShop E-commerce - ASP.NET Core Best Practices

## Overview

Best practices and coding standards for ASP.NET Core development in the ClothingShop E-commerce project. Follow these rules to maintain consistency, security, and code quality across all services.

**Project Architecture**: Clean Architecture with 4 layers

- **API Layer**: Controllers, Middleware, Program.cs configuration
- **Application Layer**: Services, DTOs, Business Logic
- **Domain Layer**: Entities, Enums, Domain Interfaces
- **Infrastructure Layer**: Repositories, Database Context, External Services

---

## 1. API Design (Controller-based)

### Controller Organization

- Always use `[Route("api/[controller]")]` pattern for API routes
- Use `[ApiController]` attribute on all controller classes
- Organize controllers by domain entity: AuthController, ProductController, CategoryController, etc.
- Add XML comments with descriptive summaries for all public API endpoints
- Always validate `ModelState.IsValid` before processing requests
- Return appropriate HTTP status codes: 200 (OK), 201 (Created), 400 (BadRequest), 401 (Unauthorized), 404 (NotFound)

### Request/Response Handling

- Always use `ApiResponse<T>` wrapper for consistent API responses
- Return `StatusCode(response.Status, response)` for flexibility
- Use `[FromBody]` for POST/PUT request bodies
- Use `[FromQuery]` for query parameters (pagination, filtering)
- Use `[FromRoute]` for path parameters (entity IDs)
- Always handle both success and failure cases in controller actions

### Controller Examples

```csharp
[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _authService.RegisterAsync(request);
        if (!response.Success)
            return BadRequest(response);
        return StatusCode(response.Status, response);
    }
}
```

---

## 2. Response Consistency

### ApiResponse Pattern

- Always wrap service responses in `ApiResponse<T>` type
- Use factory methods: `SuccessResponse()`, `FailureResponse()`, `SuccessPagedResponse()`
- Include meaningful messages in Vietnamese for user understanding
- Set appropriate HTTP status codes using `HttpStatusCode` enum
- Use `PagedResult<T>` for paginated data with metadata

### Response Structure

```csharp
public class ApiResponse<T>
{
    public int Status { get; set; }           // HTTP status code
    public bool Success { get; set; }         // Operation success flag
    public string? Message { get; set; }      // User-friendly message (Vietnamese)
    public T? Data { get; set; }             // Response data
    public string? Errors { get; set; }      // Error details
}
```

### Response Examples

- **Success (default 200 OK)**: `ApiResponse<T>.SuccessResponse(data, "Thành công")`
- **Success (with custom status)**: `ApiResponse<T>.SuccessResponse(data, "Tạo thành công", HttpStatusCode.Created)`
- **Failure**: `ApiResponse<T>.FailureResponse("Lỗi xảy ra", HttpStatusCode.BadRequest)`
- **Paginated**: `ApiResponse<PagedResult<T>>.SuccessPagedResponse(items, totalItems, pageNumber, pageSize, "Thành công")`

**Important Notes**:

- `SuccessResponse()` default status is `HttpStatusCode.OK` (200) - only specify if different
- `FailureResponse()` takes only 2 parameters: error message and HttpStatusCode
- Never use a second "message" parameter in `FailureResponse()` - it's been removed
- Use `SuccessPagedResponse()` helper method for paginated data instead of manually creating `PagedResult<T>`

---

## 3. Error Handling

### Service Layer Error Handling

- Return `ApiResponse<T>.FailureResponse()` instead of throwing exceptions for business logic errors
- Use try-catch blocks for unexpected exceptions only
- Always log errors with structured logging using `ILogger<T>`
- Never expose sensitive error details to clients in production
- Validate input data before processing in service layer

### Exception Handling Pattern

````csharp
public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
{
    try
    {
        // Business logic validation
        var user = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email);
        if (user == null)
            return ApiResponse<LoginResponse>.FailureResponse(
                "Email hoặc mật khẩu không đúng",
                HttpStatusCode.Unauthorized);

        // Success case (default 200 OK)
        return ApiResponse<LoginResponse>.SuccessResponse(
            loginResponse,
            "Đăng nhập thành công");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Lỗi khi đăng nhập với email {Email}", request.Email);
        return ApiResponse<LoginResponse>.FailureResponse(
            "Đã xảy ra lỗi hệ thống",
---

## 4. Validation

### Data Annotations

- Always use Data Annotations on DTOs for validation
- Provide meaningful error messages in Vietnamese
- Use appropriate validation attributes: `[Required]`, `[EmailAddress]`, `[Phone]`, `[MinLength]`, `[MaxLength]`, `[StringLength]`, `[Range]`
- Validate `ModelState.IsValid` in controller before calling service methods
- Return `BadRequest(ModelState)` for validation errors

### Validation Attributes Examples

```csharp
public class RegisterRequest
{
    [Required(ErrorMessage = "Họ và tên là bắt buộc")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Họ tên phải từ 3 đến 100 ký tự")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string PhoneNumber { get; set; } = string.Empty;
}
````

### Additional Validation Rules

- Validate business rules in service layer, not just data format
- Check for duplicate entries (email, phone number) before creating records
- Validate foreign key relationships exist before creating associations
- Use custom validation attributes for complex validation logic when needed

---

## 5. Security Best Practices

### Password Handling

- Always hash passwords using `IPasswordHasher` interface before storing
- Never store plain text passwords in database
- Use strong password requirements: minimum 6 characters (consider increasing to 8+)
- Validate password strength in service layer before hashing
- Implement password change functionality with old password verification

### JWT Authentication

- Store JWT configuration in `appsettings.json`, never hardcode
- Configure token validation parameters: ValidateIssuer, ValidateAudience, ValidateLifetime, ValidateIssuerSigningKey
- Use appropriate token expiration times (15-60 minutes for access tokens)
- Implement refresh token mechanism for long-lived sessions
- Store refresh tokens securely in database
- Revoke refresh tokens on logout

### JWT Configuration Example

```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true; // Always true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
```

### Authorization

- Use `[Authorize]` attribute on protected endpoints
- Implement role-based authorization using `[Authorize(Roles = "Admin")]` when needed
- Use `ICurrentUserService` to get authenticated user information
- Always validate user permissions in service layer, not just in API layer
- Never trust client-side authentication state

### Email Security

- Use secure SMTP settings with SSL/TLS
- Store email credentials in User Secrets (Development) or environment variables (Production)
- Implement rate limiting for password reset and registration emails
- Generate secure random tokens for password reset links

---

## 6. Dependency Injection

### Service Registration in Program.cs

- Register all services in `Program.cs` using `builder.Services`
- Use appropriate service lifetimes:
  - **Scoped**: Services with database context (`IUnitOfWork`, `IAuthService`, `IProductService`, etc.)
  - **Transient**: Stateless services without dependencies on DbContext
  - **Singleton**: Thread-safe services (`IPasswordHasher`, configuration services)
- Group related service registrations together with comments

### Service Registration Pattern

```csharp
// 4. ĐĂNG KÝ DI (Dependency Injection)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Authentication & Security
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Email Service
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

// User Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IUserService, UserService>();

// Domain Services
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IProductService, ProductService>();

// External Services
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddScoped<IPhotoService, PhotoService>();
```

### Constructor Injection

- Use constructor injection for all dependencies
- Always inject interfaces, never concrete classes
- Keep constructor parameter lists manageable (max 5-7 parameters)
- Consider refactoring if too many dependencies (may indicate single responsibility violation)
- Store injected dependencies in private readonly fields

### Constructor Injection Example

```csharp
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IConfiguration configuration,
        IEmailService emailService,
        ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _emailService = emailService;
        _logger = logger;
    }
}
```

---

## 7. Repository Pattern & Unit of Work

### Repository Interface Structure

- Define repository interfaces in `ClothingShop.Infrastructure/Interfaces`
- Extend `IGenericRepository<T>` for entity-specific repositories
- Use generic repository pattern for common CRUD operations
- Add specific methods only when needed (complex queries, joins, etc.)
- All repositories are accessed through `IUnitOfWork` interface

### Generic Repository Interface

```csharp
public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> FindListAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    Task UpdateAsync(T entity);
    void Delete(T entity);
    void SoftDelete(T entity);
}
```

### Entity-Specific Repository Example

```csharp
public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> IsEmailExistsAsync(string email);
    Task<User?> GetUserWithRolesAsync(Guid userId);
}
```

### Unit of Work Pattern

- Use `IUnitOfWork` to access all repositories
- Call `SaveChangesAsync()` to commit all changes in one transaction
- Dispose of UnitOfWork automatically (implements IDisposable)
- Never call `SaveChangesAsync()` on individual repositories

### Unit of Work Interface

```csharp
public interface IUnitOfWork : IDisposable
{
    // Domain Repositories
    IUserRepository Users { get; }
    IAddressRepository Addresses { get; }
    IRoleRepository Roles { get; }
    IPasswordResetHistoryRepository PasswordResets { get; }
    ICategoryRepository Categories { get; }
    IBrandRepository Brands { get; }
    IProductRepository Products { get; }

    // Save all changes in single transaction
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

### Database Operations Best Practices

- Always use async methods: `GetAllAsync`, `FindAsync`, `AddAsync`, `UpdateAsync`, `SaveChangesAsync`
- Use `FindAsync()` for single records with predicate
- Use `FindListAsync()` for multiple records with predicate
- Use transactions (via UnitOfWork) for operations that must succeed together
- Never use synchronous database methods (`.Result`, `.Wait()`)

---

## 8. Logging

### Structured Logging

- Always use `ILogger<T>` for logging in all services and controllers
- Use structured logging with named parameters, never string interpolation
- Include relevant context in log messages (email, user ID, product ID, etc.)
- Use appropriate log levels based on severity and environment

### Log Levels

- **LogError**: Exceptions, critical errors, data corruption
- **LogWarning**: Business logic issues, validation failures, recoverable errors
- **LogInformation**: Important business events (user registration, login, purchase)
- **LogDebug**: Detailed debugging information (development only)

### Logging Examples

```csharp
// Good - Structured logging with parameters
_logger.LogInformation("User registered successfully: {Email}", request.Email);
_logger.LogError(ex, "Error registering user with email {Email}", request.Email);
_logger.LogWarning("Login attempt failed for email {Email}", request.Email);

// Bad - String interpolation
_logger.LogInformation($"User registered successfully: {request.Email}");
```

### What to Log

- **Authentication events**: Login, logout, registration, password changes
- **Business transactions**: Orders, payments, cart operations
- **Errors and exceptions**: All caught exceptions with stack traces
- **Security events**: Failed login attempts, unauthorized access
- **Performance issues**: Slow queries, timeouts
- **Configuration issues**: Missing settings, invalid configurations

### What NOT to Log

- **Passwords**: Never log plain text or hashed passwords
- **Sensitive data**: Credit card numbers, API keys, secrets
- **Personal data**: Unless required for debugging (minimize in production)

---

## 9. Async/Await Patterns

### Asynchronous Methods

- Always use async/await for all I/O operations: database, HTTP, email, file system
- Return `Task<T>` or `Task` from async methods, never `void`
- Use `async Task` for methods without return value
- Never use `.Result` or `.Wait()` on async methods (causes deadlocks)
- Use async all the way: if a method calls async, it should be async

### Async Naming Convention

- Always suffix async methods with `Async`: `RegisterAsync`, `LoginAsync`, `GetProductAsync`
- Use async suffix consistently across all layers: controllers, services, repositories

### Async Best Practices

```csharp
// Good - Proper async/await
public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request)
{
    var existingUser = await _unitOfWork.Users.FindAsync(u => u.Email == request.Email);
    if (existingUser != null)
        return ApiResponse<RegisterResponse>.FailureResponse("Email đã tồn tại");

    await _unitOfWork.Users.AddAsync(newUser);
    await _unitOfWork.SaveChangesAsync();
    return ApiResponse<RegisterResponse>.SuccessResponse(response, "Đăng ký thành công");
}

// Bad - Using .Result (deadlock risk)
public Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request)
{
    var user = _unitOfWork.Users.FindAsync(u => u.Email == request.Email).Result;
    // ...
}

// Bad - Not awaiting async calls
public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request)
{
    _unitOfWork.Users.AddAsync(newUser); // Missing await
    _unitOfWork.SaveChangesAsync(); // Missing await
}
```

### ConfigureAwait

- Generally not needed in ASP.NET Core (no synchronization context)
- Only use `ConfigureAwait(false)` in library code or performance-critical scenarios
- Avoid unless you have specific reasoning

---

## 10. Clean Architecture Layers

### Layer Responsibilities

#### Domain Layer (`ClothingShop.Domain`)

- **Entities**: Core business objects (User, Product, Order, Cart, etc.)
- **Enums**: Business enumerations (OrderStatus, PaymentStatus, UserRole, etc.)
- **Domain Interfaces**: Repository interfaces only
- **NO DEPENDENCIES**: This layer should not reference any other layer
- **BaseEntity**: Common properties (Id, CreatedAt, LastModifiedAt, IsDeleted)

#### Application Layer (`ClothingShop.Application`)

- **Services**: Business logic implementation (AuthService, ProductService, etc.)
- **Service Interfaces**: Service contracts (IAuthService, IProductService, etc.)
- **DTOs**: Data transfer objects for requests and responses
- **Mappings**: Entity to DTO mapping logic
- **Wrapper Classes**: ApiResponse, PagedResult
- **DEPENDENCIES**: Can reference Domain layer only

#### Infrastructure Layer (`ClothingShop.Infrastructure`)

- **DbContext**: Entity Framework database context
- **Repositories**: Data access implementations
- **Migrations**: Database schema migrations
- **External Services**: Email, SMS, Cloud storage (Cloudinary), Payment gateways
- **DEPENDENCIES**: Can reference Domain layer only

#### API Layer (`ClothingShop.API`)

- **Controllers**: HTTP request/response handling
- **Middleware**: Cross-cutting concerns (authentication, error handling)
- **Program.cs**: Application startup, DI configuration, middleware pipeline
- **Configuration**: appsettings.json, environment-specific settings
- **DEPENDENCIES**: Can reference Application and Infrastructure layers

### Dependency Direction

```
API Layer ──────────> Application Layer ──────────> Domain Layer
     └──────────────────> Infrastructure Layer ────────┘
```

### Rules

- Keep Domain layer completely independent (no external dependencies)
- Never reference Infrastructure layer from Application layer
- API layer orchestrates Application and Infrastructure layers
- Use interfaces in Domain/Application, implementations in Infrastructure

---

## 11. Entity Framework & Database

### Entity Design

- Always inherit from `BaseEntity` for common properties
- Use `Guid` for primary keys (better for distributed systems)
- Use nullable reference types (`string?`) appropriately
- Configure relationships explicitly in `DbContext.OnModelCreating`
- Use navigation properties for related entities

### BaseEntity Pattern

```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}
```

### Entity Example

```csharp
public class Product : BaseEntity
{
    [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    public decimal Price { get; set; }

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public Guid BrandId { get; set; }
    public Brand Brand { get; set; } = null!;

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
}
```

### Database Context

- Use MySQL as the database provider (`UseMySql`)
- Configure connection string in `appsettings.json`
- Apply migrations on application startup in Development
- Use `ServerVersion.AutoDetect()` for MySQL version detection

### Database Context Configuration

```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ClothingShopDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
```

### Migrations

- Create migrations for all schema changes: `dotnet ef migrations add MigrationName`
- Review migration files before applying
- Apply migrations: `dotnet ef database update`
- Never modify database schema manually
- Use descriptive migration names (e.g., `AddProductImagesTable`, `UpdateUserEmailIndex`)

### Soft Delete Pattern

- Use `IsDeleted` flag from `BaseEntity` for soft deletes
- Implement `SoftDelete()` method in generic repository
- Filter out soft-deleted records in queries using global query filters
- Provide admin functionality to permanently delete if needed

---

## 12. External Services Integration

### Email Service (SMTP)

- Use `IEmailService` interface for email operations
- Configure SMTP settings in `appsettings.json`
- Store credentials in User Secrets (Development) or environment variables (Production)
- Implement async email sending: `SendEmailAsync()`
- Handle email failures gracefully (log and return failure response)

### Email Configuration

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "EnableSsl": true,
    "SenderEmail": "noreply@clothingshop.com",
    "SenderName": "ClothingShop",
    "Username": "stored-in-user-secrets",
    "Password": "stored-in-user-secrets"
  }
}
```

### Photo Service (Cloudinary)

- Use `IPhotoService` interface for image operations
- Configure Cloudinary settings in `appsettings.json`
- Store credentials securely (User Secrets or environment variables)
- Upload product images, user avatars using `UploadPhotoAsync()`
- Delete images when products/users are deleted using `DeletePhotoAsync()`
- Store image URLs in database, not the actual images

### Cloudinary Configuration

```csharp
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddScoped<IPhotoService, PhotoService>();
```

### External Service Best Practices

- Always use interfaces for external services (testability)
- Handle service failures gracefully with try-catch and logging
- Implement retry logic for transient failures when appropriate
- Never expose external service credentials to clients
- Use async methods for all I/O operations
- Log all external service interactions

---

## 13. Configuration Management

### appsettings.json Structure

- Store all configuration in `appsettings.json` and `appsettings.Development.json`
- Use `appsettings.Development.json` for local development settings
- Never commit sensitive data (passwords, API keys) to source control
- Use hierarchical configuration structure with sections

### Configuration Sections

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ClothingShopDB;..."
  },
  "Jwt": {
    "Key": "your-secret-key-stored-in-user-secrets",
    "Issuer": "ClothingShop",
    "Audience": "ClothingShop-Users",
    "AccessTokenExpirationMinutes": 30,
    "RefreshTokenExpirationDays": 7
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "EnableSsl": true,
    "SenderEmail": "noreply@clothingshop.com",
    "SenderName": "ClothingShop"
  },
  "Cloudinary": {
    "CloudName": "your-cloud-name",
    "ApiKey": "stored-in-user-secrets",
    "ApiSecret": "stored-in-user-secrets"
  }
}
```

### User Secrets (Development)

- Use User Secrets for sensitive data in development
- Initialize: `dotnet user-secrets init`
- Set secrets: `dotnet user-secrets set "Jwt:Key" "your-secret-key"`
- Never commit secrets to Git

### Environment Variables (Production)

- Use environment variables for production secrets
- Configure in hosting environment (Azure, AWS, Docker, etc.)
- Access via `IConfiguration` interface

### Strongly-Typed Configuration

- Create configuration classes for complex settings
- Use `IOptions<T>` pattern for dependency injection
- Bind configuration sections to classes using `builder.Configuration.GetSection()`

---

## 14. CORS Configuration

### CORS Setup

- Configure CORS in `Program.cs` before building the app
- Allow specific origins in production, avoid `AllowAnyOrigin()` in production
- Allow necessary HTTP methods: GET, POST, PUT, DELETE, PATCH
- Allow necessary headers: Authorization, Content-Type
- Allow credentials if using cookies or authentication

### CORS Configuration Example

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://clothingshop.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// In middleware pipeline (after routing, before authorization)
app.UseCors("AllowFrontend");
```

### CORS Best Practices

- Use named CORS policies for better organization
- Never use `AllowAnyOrigin()` in production
- Be specific about allowed origins, methods, and headers
- Test CORS configuration thoroughly with frontend
- Document allowed origins in README.md

---

## 15. Swagger / OpenAPI Configuration

### Swagger Setup

- Configure Swagger in `Program.cs` for API documentation
- Add JWT authentication support in Swagger UI
- Use XML comments for better documentation (optional but recommended)
- Enable Swagger only in Development environment for security

### Swagger Configuration with JWT

```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ClothingShop API",
        Version = "v1",
        Description = "E-commerce API for ClothingShop",
        Contact = new OpenApiContact
        {
            Name = "ClothingShop Team",
            Email = "support@clothingshop.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Nhập JWT Token của bạn vào đây (không cần gõ chữ Bearer)"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Enable Swagger middleware (Development only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClothingShop API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}
```

---

## 16. Code Organization & Naming

### Project Structure

```
ClothingShop.API/
├── Controllers/          # API controllers by domain
├── Properties/           # Launch settings
├── Program.cs           # Application startup
├── appsettings.json     # Configuration

ClothingShop.Application/
├── DTOs/                # Request/Response DTOs by domain
│   ├── Auth/
│   ├── Product/
│   └── User/
├── Services/            # Business logic services
│   ├── Auth/
│   │   ├── Interfaces/
│   │   └── Impl/
│   └── ProductService/
├── Wrapper/             # Response wrappers (ApiResponse, PagedResult)

ClothingShop.Domain/
├── Entities/            # Domain entities
├── Enums/              # Business enumerations

ClothingShop.Infrastructure/
├── Interfaces/          # Repository interfaces
├── Repositories/        # Repository implementations
├── Persistence/         # UnitOfWork, DbContext
├── Migrations/          # EF Core migrations
├── Services/           # External service implementations
└── Templates/          # Email templates
```

### Naming Conventions

- **PascalCase**: Classes, methods, properties, interfaces, namespaces
- **camelCase**: Local variables, method parameters, private fields
- **Prefix interfaces with I**: `IAuthService`, `IProductRepository`, `IUnitOfWork`
- **Async suffix for async methods**: `RegisterAsync`, `GetProductAsync`, `SaveChangesAsync`
- **Descriptive names**: Clearly indicate purpose and responsibility

### File Organization

- One class per file (except DTOs which can be grouped)
- File name matches class name exactly
- Group related files in folders by domain (Auth, Product, User, etc.)
- Separate interfaces from implementations (use Interfaces/ and Impl/ folders)
- Keep namespaces matching folder structure

### DTO Organization

- Group DTOs by domain entity (Auth, Product, User, etc.)
- Use descriptive suffixes: `Request`, `Response`, `CreateDto`, `UpdateDto`
- Place related DTOs in same folder
- Examples: `RegisterRequest`, `LoginResponse`, `ProductCreateDto`, `UserResponse`

---

## 17. Testing Considerations

### Testability Design

- Design services to be testable using dependency injection
- Always inject interfaces, never concrete classes
- Keep business logic separate from infrastructure concerns
- Avoid static methods and dependencies
- Use repository pattern for data access abstraction

### Test Types

- **Unit Tests**: Test business logic in services with mocked dependencies
- **Integration Tests**: Test database operations with real DbContext
- **API Tests**: Test controller endpoints with test server
- **End-to-End Tests**: Test complete user workflows

### Mocking

- Mock `IUnitOfWork` and repositories in service unit tests
- Mock external services (`IEmailService`, `IPhotoService`) in tests
- Use mocking frameworks like Moq or NSubstitute
- Never mock what you don't own (e.g., DbContext) - use integration tests instead

---

## 18. Performance Best Practices

### Database Query Optimization

- Use `FindAsync()` instead of fetching all records and filtering in memory
- Include only necessary data in queries (use Select for projections)
- Avoid N+1 query problems using eager loading (`.Include()`)
- Use pagination for large datasets with `PagedResult<T>`
- Create database indexes for frequently queried columns (email, foreign keys)

### Performance Examples

```csharp
// Good - Query database with predicate
var user = await _unitOfWork.Users.FindAsync(u => u.Email == email);

// Bad - Load all users into memory then filter
var allUsers = await _unitOfWork.Users.GetAllAsync();
var user = allUsers.FirstOrDefault(u => u.Email == email);

// Good - Eager loading to avoid N+1
var products = await _unitOfWork.Products
    .FindListAsync(p => p.CategoryId == categoryId);
// Then configure Include in repository if needed

// Good - Pagination
var pagedProducts = await _productService.GetProductsAsync(pageNumber: 1, pageSize: 20);
```

### Caching

- Cache frequently accessed data that doesn't change often (categories, brands)
- Use in-memory caching (`IMemoryCache`) for small datasets
- Use distributed caching (Redis) for scalability in production
- Set appropriate cache expiration times
- Invalidate cache when data changes

### Async Operations

- Use async/await for all I/O operations to improve scalability
- Never block on async code (`.Result`, `.Wait()`)
- Use `Task.WhenAll()` for parallel independent operations

---

## 19. Security Checklist

### Authentication & Authorization

- [x] JWT tokens configured with secure settings
- [x] Token expiration times set appropriately
- [x] Refresh token mechanism implemented
- [x] Password hashing using secure algorithm (not plain MD5)
- [x] `[Authorize]` attribute on protected endpoints
- [x] User roles and permissions validated in service layer

### Data Protection

- [x] Never log passwords (plain text or hashed)
- [x] Never expose sensitive data in error messages
- [x] Validate and sanitize all user inputs
- [x] Use parameterized queries (EF Core does this by default)
- [x] Implement rate limiting for authentication endpoints
- [x] HTTPS enforced in production (`RequireHttpsMetadata = true`)

### Configuration Security

- [x] No hardcoded secrets in source code
- [x] Use User Secrets for development
- [x] Use environment variables for production
- [x] Connection strings secured
- [x] CORS configured properly (no `AllowAnyOrigin()` in production)

---

## 20. Quick Reference

### Common Patterns

```csharp
// Service Success Response (default 200 OK)
return ApiResponse<T>.SuccessResponse(data, "Thành công");

// Service Success Response (with custom status code)
return ApiResponse<T>.SuccessResponse(data, "Tạo thành công", HttpStatusCode.Created);

// Service Error Response
return ApiResponse<T>.FailureResponse("Lỗi xảy ra", HttpStatusCode.BadRequest);

// Paginated Response (using helper method)
return ApiResponse<PagedResult<T>>.SuccessPagedResponse(
    items, totalItems, pageNumber, pageSize, "Thành công");

// Structured Logging
_logger.LogInformation("User {UserId} performed action {Action}", userId, action);

// Async Query
var user = await _unitOfWork.Users.FindAsync(u => u.Email == email);

// Service Registration
builder.Services.AddScoped<IAuthService, AuthService>();

// Controller Action
[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterRequest request)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var response = await _authService.RegisterAsync(request);
    if (!response.Success)
        return BadRequest(response);
    return StatusCode(response.Status, response);
}
```

### Service Lifetimes

- **Scoped**: `IUnitOfWork`, `IAuthService`, `IProductService`, `IEmailService`, `IPhotoService`
- **Transient**: Stateless services without DbContext dependencies
- **Singleton**: `IPasswordHasher`, configuration readers, in-memory cache

---

## 21. Pre-Commit Checklist

Before committing code, ensure:

### API & Response Handling

- [ ] All controller actions return `IActionResult`
- [ ] All service methods return `ApiResponse<T>`
- [ ] Services return `FailureResponse()` instead of throwing exceptions for business errors
- [ ] Appropriate HTTP status codes returned (200, 400, 401, 404, 500)
- [ ] `ModelState.IsValid` validated in all POST/PUT endpoints

### Async & Database

- [ ] All async methods use async/await properly, no `.Result` or `.Wait()`
- [ ] All database operations use async methods
- [ ] Repository pattern and Unit of Work used for all data access
- [ ] `SaveChangesAsync()` called to commit changes

### Validation & Security

- [ ] DTOs have validation attributes with meaningful Vietnamese error messages
- [ ] Password hashing implemented correctly (never plain text)
- [ ] JWT authentication configured properly
- [ ] `[Authorize]` attribute on protected endpoints
- [ ] User permissions validated in service layer

### Logging & Error Handling

- [ ] Logging uses structured logging with parameters, not string interpolation
- [ ] Exceptions caught and logged appropriately
- [ ] No sensitive data logged (passwords, tokens, etc.)
- [ ] Error messages user-friendly in Vietnamese

### Architecture & Dependencies

- [ ] Clean architecture layers respected (no circular dependencies)
- [ ] Dependencies injected through constructors using interfaces
- [ ] Configuration stored in `appsettings.json`, not hardcoded
- [ ] No secrets committed to source control

### Code Quality

- [ ] Code follows consistent naming conventions (PascalCase, camelCase)
- [ ] Files organized properly in domain folders
- [ ] XML comments added for public APIs (optional but recommended)
- [ ] Code reviewed for performance issues (N+1 queries, memory leaks)

---

## 22. Common Mistakes to Avoid

### ❌ DON'T

```csharp
// Don't use .Result or .Wait()
var user = _unitOfWork.Users.FindAsync(u => u.Email == email).Result;

// Don't use string interpolation in logging
_logger.LogInformation($"User {userId} logged in");

// Don't throw exceptions for business logic errors
if (user == null)
    throw new Exception("User not found");

// Don't return raw exceptions to clients
catch (Exception ex)
{
    return StatusCode(500, ex.Message);
}

// Don't hardcode configuration
var jwtKey = "my-super-secret-key";

// Don't inject concrete classes
public AuthService(UnitOfWork unitOfWork) { }

// Don't load all data and filter in memory
var allProducts = await _unitOfWork.Products.GetAllAsync();
var filteredProducts = allProducts.Where(p => p.CategoryId == categoryId).ToList();
```

### ✅ DO

```csharp
// Do use async/await properly
var user = await _unitOfWork.Users.FindAsync(u => u.Email == email);

// Do use structured logging
_logger.LogInformation("User {UserId} logged in", userId);

// Do return failure responses for business logic errors (only 2 parameters)
if (user == null)
    return ApiResponse<LoginResponse>.FailureResponse(
        "Email hoặc mật khẩu không đúng",
        HttpStatusCode.Unauthorized);

// Do handle exceptions gracefully
catch (Exception ex)
{
    _logger.LogError(ex, "Error during login");
    return ApiResponse<LoginResponse>.FailureResponse(
        "Đã xảy ra lỗi hệ thống",
        HttpStatusCode.InternalServerError);
}

// Do use configuration
var jwtKey = _configuration["Jwt:Key"];

// Do inject interfaces
public AuthService(IUnitOfWork unitOfWork) { }

// Do query database with predicates
var products = await _unitOfWork.Products.FindListAsync(p => p.CategoryId == categoryId);

// Do use SuccessPagedResponse for paginated data
return ApiResponse<PagedResult<UserDto>>.SuccessPagedResponse(
    userDtos, totalCount, pageNumber, pageSize, "Lấy danh sách thành công");
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core)
- [Clean Architecture by Uncle Bob](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

### Project-Specific

- Review existing code in `ClothingShop.Application/Services/Auth` for examples
- Check `ClothingShop.API/Controllers` for controller patterns
- See `ClothingShop.Infrastructure/Repositories` for repository implementations

---

## Version History

- **v1.1** - Updated Wrapper classes (ApiResponse, PagedResult) with correct usage patterns
  - Removed redundant 2nd parameter from FailureResponse
  - Added SuccessPagedResponse helper method
  - Added HasPreviousPage and HasNextPage to PagedResult
  - Updated all examples and common patterns
  - Date: January 19, 2026
- **v1.0** - Initial ClothingShop coding standards based on existing project structure
  - Date: January 19, 2026
```

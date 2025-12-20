using System.Security.Claims;
using ClothingShop.Application.Services.Auth.Interfaces;
using ClothingShop.Domain.Entities;
using ClothingShop.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork; // Hoặc DbContext nếu bạn làm đơn giản

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
    {
        _httpContextAccessor = httpContextAccessor;
        _unitOfWork = unitOfWork;
    }

    public Guid? UserId
    {
        get
        {
            var id = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return string.IsNullOrEmpty(id) ? null : Guid.Parse(id);
        }
    }

    // Lấy object User đầy đủ kèm theo Role
    public async Task<User?> GetUserAsync()
    {
        var id = UserId;
        if (id == null) return null;

        // Truy vấn DB để lấy User cùng các quan hệ (như Role)
        return await _unitOfWork.Users.GetByIdAsync(id.Value);
        // Hoặc: return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
    }
}
using ClothingShop.Domain.Entities;

namespace ClothingShop.Application.Services.Auth.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        Task<User?> GetUserAsync();
    }
}

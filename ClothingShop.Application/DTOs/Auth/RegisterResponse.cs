using ClothingShop.Application.DTOs.User;

namespace ClothingShop.Application.DTOs.Auth;

public class RegisterResponse
{
    public UserAuthDto User { get; set; } = null!;
}

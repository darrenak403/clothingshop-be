using ClothingShop.Application.DTOs.User;

namespace ClothingShop.Application.DTOs.Auth;

public class RegisterResponse
{
    public UserDto User { get; set; } = null!;
}

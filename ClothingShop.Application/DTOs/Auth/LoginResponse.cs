using ClothingShop.Application.DTOs.User;

namespace ClothingShop.Application.DTOs.Auth;

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public UserAuthDto User { get; set; } = null!;
}

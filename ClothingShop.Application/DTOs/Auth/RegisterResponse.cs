using System;

namespace ClothingShop.Application.DTOs.Auth;

public class RegisterResponse
{
    public UserDto User { get; set; } = null!;
}

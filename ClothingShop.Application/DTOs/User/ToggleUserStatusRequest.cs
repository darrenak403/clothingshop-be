namespace ClothingShop.Application.DTOs.User
{
    public class ToggleUserStatusRequest
    {
        public bool IsActive { get; set; }
        public string? Reason { get; set; }
    }
}

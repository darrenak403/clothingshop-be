namespace ClothingShop.Application.DTOs.User
{
    public class UserFilterRequest
    {
        public string? Keyword { get; set; }
        public bool? IsActive { get; set; }
        public int? RoleId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
namespace ClothingShop.Application.DTOs.User
{
    public class UserFilterRequest
    {
        public string? SearchTerm { get; set; } // Tìm theo tên/email
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

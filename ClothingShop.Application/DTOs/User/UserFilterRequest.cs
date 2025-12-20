namespace ClothingShop.Application.DTOs.User
{
    public class UserFilterRequest
    {
        public string? Keyword { get; set; } // Tìm theo tên hoặc email
        public bool? IsActive { get; set; }  // Lọc theo trạng thái
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
namespace ClothingShop.Application.DTOs.User
{
    public class UserDto : UserProfileDto
    {
        public bool IsActive { get; set; } // Admin cần biết trạng thái
        public string? LockReason { get; set; } // Lý do khóa
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }

        // Thông tin thống kê (Computed properties)
        public int TotalOrders { get; set; } // Tổng số đơn hàng user đã đặt
        public decimal? TotalSpent { get; set; } // Tổng tiền user đã tiêu (Optional - nâng cao)
    }
}
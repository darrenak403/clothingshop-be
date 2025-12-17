namespace ClothingShop.Domain.Entities
{
    public class Voucher : BaseEntity
    {
        public string Code { get; set; } = null!; // SALE50
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        // Logic giảm giá
        public bool IsPercentage { get; set; } = false; // True: % | False: Số tiền
        public decimal Value { get; set; } // 10 (%) hoặc 50000 (VND)
        public decimal? MaxDiscountAmount { get; set; } // Giảm tối đa 100k (cho loại %)
        public decimal MinOrderAmount { get; set; } // Đơn tối thiểu 200k

        // Thời hạn & Số lượng
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Quantity { get; set; } // Tổng số mã phát hành
        public int UsedCount { get; set; } = 0; // Số mã đã dùng

        public bool IsActive { get; set; } = true;
    }
}

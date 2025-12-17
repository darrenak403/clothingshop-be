namespace ClothingShop.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public string PaymentMethod { get; set; } = null!; // VNPay, Momo, Cash
        public string? TransactionId { get; set; } // Mã giao dịch ngân hàng
        public decimal Amount { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string? ResponseMessage { get; set; } // Lời nhắn từ cổng thanh toán
    }
}

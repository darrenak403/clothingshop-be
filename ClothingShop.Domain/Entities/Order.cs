using ClothingShop.Domain.Enums;

namespace ClothingShop.Domain.Entities
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string OrderCode { get; set; } = null!; // Mã đơn: #ORD251217-001 (Dễ đọc)

        // Snapshot Address (Lưu cứng tại thời điểm đặt)
        public string ShippingAddress { get; set; } = null!;
        public string ShippingCity { get; set; } = null!;
        public string ShippingDistrict { get; set; } = null!;
        public string ShippingWard { get; set; } = null!;
        public string ShippingPhone { get; set; } = null!;
        public string RecipientName { get; set; } = null!;
        public string? Note { get; set; } // Ghi chú của khách

        // Money
        public decimal SubTotal { get; set; } // Tổng tiền hàng
        public decimal ShippingFee { get; set; } // Phí ship
        public decimal DiscountAmount { get; set; } // Giảm giá
        public decimal TotalAmount { get; set; } // = SubTotal + Ship - Discount

        // Status
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

        // Logistics
        public string? TrackingNumber { get; set; } // Mã vận đơn GHTK/GHN
        public string? Carrier { get; set; } // Đơn vị vận chuyển
        public DateTime? ShippedDate { get; set; }

        public ICollection<OrderDetail> OrderItems { get; set; } = new List<OrderDetail>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}

namespace ClothingShop.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 0,    // Mới đặt, chờ xác nhận
        Confirmed = 1,  // Đã xác nhận
        Packing = 2,    // Đang đóng gói
        Shipping = 3,   // Đang giao
        Delivered = 4,  // Giao thành công
        Cancelled = 5,  // Hủy
        Returned = 6    // Trả hàng/Hoàn tiền
    }

    public enum PaymentMethod
    {
        COD = 0,        // Tiền mặt khi nhận
        Banking = 1,    // Chuyển khoản ngân hàng
        VNPay = 2,
        Momo = 3
    }

    public enum PaymentStatus
    {
        Unpaid = 0,
        Paid = 1,
        Refunded = 2
    }

    public enum AttemptStatus
    {
        Pending = 0,
        Used = 1,
        Expired = 2,
        Failed = 3,
    }
}

namespace ClothingShop.Domain.Entities
{
    public class OrderDetail
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public Guid ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; } = null!;

        // Snapshot Product Info (Lưu cứng tên/giá phòng khi sản phẩm bị sửa/xóa)
        public string ProductName { get; set; } = null!;
        public string ProductSku { get; set; } = null!;
        public string Size { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string? Thumbnail { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // Giá tại thời điểm mua
        public decimal TotalPrice { get; set; }
    }
}

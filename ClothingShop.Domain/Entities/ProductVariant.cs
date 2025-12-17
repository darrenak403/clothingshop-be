namespace ClothingShop.Domain.Entities
{
    public class ProductVariant : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        // Thuộc tính biến thể
        public string Color { get; set; } = null!; // Màu sắc (Red, Blue)
        public string? ColorCode { get; set; }     // Mã màu Hex (#FF0000) - để hiển thị chấm màu
        public string Size { get; set; } = null!;  // Kích thước (S, M, L)

        // Kho vận
        public int StockQuantity { get; set; } = 0;
        public string Sku { get; set; } = null!; // Mã quản lý kho duy nhất
        public double Weight { get; set; } = 0;  // Gram (để tính phí ship)

        // Giá riêng cho biến thể (nếu size XXL đắt hơn size S)
        public decimal? OverridePrice { get; set; }
    }
}

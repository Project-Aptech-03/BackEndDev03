using ProjectDemoWebApi.DTOs.Products;

namespace ProjectDemoWebApi.DTOs.ShoppingCart
{
    public class ShoppingCartResponseDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
        public DateTime AddedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        // Related product info
        public ProductsResponseDto? Product { get; set; }
    }

    public class CartSummaryDto
    {
        public List<ShoppingCartResponseDto> Items { get; set; } = new();
        public int TotalItems => Items.Sum(x => x.Quantity);
        public decimal SubTotal => Items.Sum(x => x.TotalPrice);
        public decimal DeliveryCharges { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount => SubTotal + DeliveryCharges - DiscountAmount;
        
        // Coupon information
        public string? AppliedCouponCode { get; set; }
        public string? CouponDiscountType { get; set; }
        public decimal? CouponDiscountValue { get; set; }
        public bool HasValidCoupon => !string.IsNullOrEmpty(AppliedCouponCode) && DiscountAmount > 0;
    }
}
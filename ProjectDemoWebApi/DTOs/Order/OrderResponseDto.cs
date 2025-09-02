using ProjectDemoWebApi.DTOs.CustomerAddress;
using ProjectDemoWebApi.DTOs.Products;
using ProjectDemoWebApi.DTOs.User;

namespace ProjectDemoWebApi.DTOs.Order
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public int DeliveryAddressId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Subtotal { get; set; }
        public decimal CouponDiscountAmount { get; set; }
        public decimal DeliveryCharges { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string? AppliedCoupons { get; set; }
        public string? DeliveryNotes { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? CancelledDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        // Related entities
        public UsersResponseDto? Customer { get; set; }
        public CustomerAddressResponseDto? DeliveryAddress { get; set; }
        public List<OrderItemResponseDto> OrderItems { get; set; } = new();
        public List<PaymentResponseDto> Payments { get; set; } = new();
    }

    public class OrderItemResponseDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Notes { get; set; }

        // Related product info
        public ProductsResponseDto? Product { get; set; }
    }

    public class PaymentResponseDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? TransactionId { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime? PaymentDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Order
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "Delivery address ID cannot be empty.")]
        public int DeliveryAddressId { get; set; }

        [Required(ErrorMessage = "Payment type cannot be empty.")]
        [StringLength(50, ErrorMessage = "Payment type cannot exceed 50 characters.")]
        public string PaymentType { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Delivery notes cannot exceed 500 characters.")]
        public string? DeliveryNotes { get; set; }

        public List<string>? CouponCodes { get; set; }

        [Required(ErrorMessage = "Order items cannot be empty.")]
        public List<CreateOrderItemDto> OrderItems { get; set; } = new();
    }

    public class CreateOrderItemDto
    {
        [Required(ErrorMessage = "Product ID cannot be empty.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity cannot be empty.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [StringLength(255, ErrorMessage = "Notes cannot exceed 255 characters.")]
        public string? Notes { get; set; }
    }
}
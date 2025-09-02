using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Order
{
    public class UpdateOrderDto
    {
        [StringLength(50, ErrorMessage = "Order status cannot exceed 50 characters.")]
        public string? OrderStatus { get; set; }

        [StringLength(50, ErrorMessage = "Payment status cannot exceed 50 characters.")]
        public string? PaymentStatus { get; set; }
    }
}
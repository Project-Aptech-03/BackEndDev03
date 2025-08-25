using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Payment
{
    public class CreatePaymentDto
    {
        [Required(ErrorMessage = "Order ID cannot be empty.")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Payment method cannot be empty.")]
        [StringLength(50, ErrorMessage = "Payment method cannot exceed 50 characters.")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount cannot be empty.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [StringLength(100, ErrorMessage = "Transaction ID cannot exceed 100 characters.")]
        public string? TransactionId { get; set; }
    }
}
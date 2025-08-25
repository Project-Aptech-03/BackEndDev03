using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Payment
{
    public class UpdatePaymentDto
    {
        [StringLength(50, ErrorMessage = "Payment status cannot exceed 50 characters.")]
        public string? PaymentStatus { get; set; }

        [StringLength(100, ErrorMessage = "Transaction ID cannot exceed 100 characters.")]
        public string? TransactionId { get; set; }

        public DateTime? PaymentDate { get; set; }
    }
}
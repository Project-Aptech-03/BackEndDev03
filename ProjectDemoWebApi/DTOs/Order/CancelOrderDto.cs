using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Order
{
    public class CancelOrderDto
    {
        [Required(ErrorMessage = "Cancellation reason is required.")]
        [StringLength(500, ErrorMessage = "Cancellation reason cannot exceed 500 characters.")]
        [MinLength(10, ErrorMessage = "Cancellation reason must be at least 10 characters.")]
        public string CancellationReason { get; set; } = string.Empty;
    }
}
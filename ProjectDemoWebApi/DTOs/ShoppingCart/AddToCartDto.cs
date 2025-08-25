using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.ShoppingCart
{
    public class AddToCartDto
    {
        [Required(ErrorMessage = "Product ID cannot be empty.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity cannot be empty.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;
    }
}
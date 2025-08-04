using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.Models
{
    public class Product
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn hoặc bằng 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Mô tả sản phẩm là bắt buộc.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hình ảnh sản phẩm là bắt buộc.")]
        public string ImageUrl { get; set; } = string.Empty;

        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }

}

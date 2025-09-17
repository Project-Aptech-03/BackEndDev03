namespace ProjectDemoWebApi.DTOs.Order
{
    public class BestSellerProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
    }
}

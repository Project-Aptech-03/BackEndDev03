namespace ProjectDemoWebApi.DTOs.Manufacturer
{
    public class ManufacturerResponseDto
    {
        public int Id { get; set; }
        public string ManufacturerCode { get; set; } = string.Empty;
        public string ManufacturerName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ProductCount { get; set; }
    }
}
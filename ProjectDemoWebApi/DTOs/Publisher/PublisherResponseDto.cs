namespace ProjectDemoWebApi.DTOs.Publisher
{
    public class PublisherResponseDto
    {
        public int Id { get; set; }
        public string PublisherName { get; set; } = string.Empty;
        public string? PublisherAddress { get; set; }
        public string? ContactInfo { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ProductCount { get; set; }
    }
}
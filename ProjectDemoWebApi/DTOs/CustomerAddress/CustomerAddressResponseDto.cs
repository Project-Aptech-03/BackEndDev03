namespace ProjectDemoWebApi.DTOs.CustomerAddress
{
    public class CustomerAddressResponseDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public string? District { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public decimal? DistanceKm { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string DisplayAddress => $"{FullAddress}, {District}, {City} {PostalCode}".Trim(' ', ',');
    }
}
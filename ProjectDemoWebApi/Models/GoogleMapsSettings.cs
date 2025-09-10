namespace ProjectDemoWebApi.Models
{
    public class GoogleMapsSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string GeocodingUrl { get; set; } = string.Empty;
        public string DistanceMatrixUrl { get; set; } = string.Empty;
    }
}
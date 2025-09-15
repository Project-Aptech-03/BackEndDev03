namespace ProjectDemoWebApi.Models
{
    public class SePaySettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string ApiUrl { get; set; } = "https://my.sepay.vn/userapi";
    }
}
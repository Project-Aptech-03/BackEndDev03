namespace ProjectDemoWebApi.DTOs.Response
{
    public class OtpResultDto
    {
        public string PendingUserId { get; set; } = default!;
        public string Email { get; set; } = default!;
        public int ExpiresIn { get; set; }
    }

}

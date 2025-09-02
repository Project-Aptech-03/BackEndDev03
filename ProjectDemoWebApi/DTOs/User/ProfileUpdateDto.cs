namespace ProjectDemoWebApi.DTOs.User
{
    public class ProfileUpdateDto
    {

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }

        public string? AvatarUrl { get; set; }
    }   
}

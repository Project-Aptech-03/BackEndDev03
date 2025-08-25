namespace ProjectDemoWebApi.DTOs.User
{
    public class UsersResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? AvataUrl { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}

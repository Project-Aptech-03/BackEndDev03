namespace ProjectDemoWebApi.DTOs.Response
{
    public class UsersResponseDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }  
        public string FullName => $"{FirstName} {LastName}";
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}

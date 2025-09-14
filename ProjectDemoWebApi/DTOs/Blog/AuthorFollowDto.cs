namespace ProjectDemoWebApi.DTOs.Blog
{
    public class AuthorFollowDto
    {
        public string AuthorId { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string? AuthorAvatar { get; set; }
        public int BlogCount { get; set; }
        public int FollowerCount { get; set; }
        public bool IsFollowing { get; set; }
    }
}

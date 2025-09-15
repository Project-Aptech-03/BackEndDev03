namespace ProjectDemoWebApi.DTOs.Blog
{
    public class CommentResponseDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public int LikeCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int BlogId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? UserAvatar { get; set; }
        public int? ParentCommentId { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public List<CommentResponseDto> Replies { get; set; } = new List<CommentResponseDto>();
    }
}

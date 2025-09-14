using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Blog
{
    public class CreateCommentDto
    {
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;

        public int? ParentCommentId { get; set; }
    }
}

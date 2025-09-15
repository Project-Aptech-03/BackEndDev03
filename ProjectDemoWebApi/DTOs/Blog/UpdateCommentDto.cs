using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Blog
{

    public class UpdateCommentDto
    {
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; } = string.Empty;
    }
}

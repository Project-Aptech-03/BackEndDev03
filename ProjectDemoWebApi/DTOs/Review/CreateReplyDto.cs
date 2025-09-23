using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Review
{
    public class CreateReplyDto
    {
        [Required(ErrorMessage = "Comment is required.")]
        [StringLength(2000, ErrorMessage = "Comment cannot exceed 2000 characters.")]
        public string Comment { get; set; } = string.Empty;

        public int? ParentReplyId { get; set; }
    }
}
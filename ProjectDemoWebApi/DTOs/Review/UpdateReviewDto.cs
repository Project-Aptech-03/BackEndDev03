using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Review
{
    public class UpdateReviewDto
    {
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int? Rating { get; set; }

        [StringLength(2000, ErrorMessage = "Comment cannot exceed 2000 characters.")]
        public string? Comment { get; set; }

        public List<IFormFile>? NewReviewImages { get; set; }
        public List<int>? ImagesToDelete { get; set; }
    }
}
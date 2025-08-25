using System.ComponentModel.DataAnnotations;

namespace ProjectDemoWebApi.DTOs.Publisher
{
    public class UpdatePublisherDto
    {
        [StringLength(150, ErrorMessage = "Publisher name cannot exceed 150 characters.")]
        public string? PublisherName { get; set; }

        [StringLength(500, ErrorMessage = "Publisher address cannot exceed 500 characters.")]
        public string? PublisherAddress { get; set; }

        [StringLength(255, ErrorMessage = "Contact info cannot exceed 255 characters.")]
        public string? ContactInfo { get; set; }

        public bool? IsActive { get; set; }
    }
}
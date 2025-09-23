using ProjectDemoWebApi.DTOs.Products;
using ProjectDemoWebApi.DTOs.User;

namespace ProjectDemoWebApi.DTOs.Review
{
    public class ReviewResponseDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string CustomerId { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public bool IsApproved { get; set; } = true;
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        // Related entities
        public UsersResponseDto? Customer { get; set; }
        public ProductsResponseDto? Product { get; set; }
        public List<ReviewImageDto> ReviewImages { get; set; } = new();
        public List<ReviewReplyDto> ReviewReplies { get; set; } = new();
    }

    public class ReviewImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class ReviewReplyDto
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public int? ParentReplyId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public bool IsAdminReply { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime ReplyDate { get; set; }
        public DateTime CreatedDate { get; set; }

        // Related user info
        public UsersResponseDto? User { get; set; }
        public List<ReviewReplyDto> ChildReplies { get; set; } = new();
    }
}
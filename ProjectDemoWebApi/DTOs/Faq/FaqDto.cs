//namespace ProjectDemoWebApi.DTOs
//{
//    public class FaqDto
//    {
//        public int Id { get; set; }
//        public string Question { get; set; }
//        public string Answer { get; set; }
//        public int DisplayOrder { get; set; }
//    }

//    public class CreateFaqDto
//    {
//        public string Question { get; set; }
//        public string Answer { get; set; }
//        public int DisplayOrder { get; set; }
//    }

//    public class UpdateFaqDto
//    {
//        public string Question { get; set; }
//        public string Answer { get; set; }
//        public int DisplayOrder { get; set; }
//        public bool IsActive { get; set; }
//    }
//}


namespace ProjectDemoWebApi.DTOs
{
    public class FaqDto
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } // THÊM VÀO
        public DateTime CreatedAt { get; set; } // THÊM VÀO
    }

    public class CreateFaqDto
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } // THÊM VÀO
    }

    public class UpdateFaqDto
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
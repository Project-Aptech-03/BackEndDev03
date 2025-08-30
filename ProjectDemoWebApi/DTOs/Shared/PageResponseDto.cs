namespace ProjectDemoWebApi.DTOs.Shared
{
    public class PagedResponseDto<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }                
        public int PageIndex { get; set; }                 
        public int PageSize { get; set; }                  
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }

}

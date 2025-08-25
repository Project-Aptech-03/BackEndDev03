
namespace ProjectDemoWebApi.DTOs.Response
    {
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public object? Errors { get; set; } = null;
        public int StatusCode { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "", int statusCode = 200)
            => new() { Success = true, Data = data, Message = message, StatusCode = statusCode };

        public static ApiResponse<T> Fail(string message, object? errors = null, int statusCode = 400)
            => new() { Success = false, Message = message, Errors = errors, StatusCode = statusCode };

        internal static object? Ok(object value, string v)
        {
            throw new NotImplementedException();
        }
    }

}
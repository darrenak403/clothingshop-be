using System.Net;

namespace ClothingShop.Application.Wrapper
{
    public class ApiResponse<T>
    {
        public int Status { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public string Errors { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success", HttpStatusCode status = HttpStatusCode.OK)
        {
            return new ApiResponse<T>
            {
                Status = (int)status,
                Success = true,
                Message = message,
                Data = data,
            };
        }

        public static ApiResponse<T> FailureResponse(string errors, string message = "Failed", HttpStatusCode status = HttpStatusCode.BadRequest)
        {
            return new ApiResponse<T>
            {
                Status = (int)status,
                Success = false,
                Message = message,
                Errors = errors,
            };
        }
    }
}

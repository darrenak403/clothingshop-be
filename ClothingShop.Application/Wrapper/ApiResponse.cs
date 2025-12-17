using System.Net;

namespace ClothingShop.Application.Wrapper
{
    public class ApiResponse<T>
    {
        public int status { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public string Errors { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                status = (int)HttpStatusCode.OK,
                Success = true,
                Message = message,
                Data = data,
            };
        }

        public static ApiResponse<T> FailureResponse(string errors, string message = "Failed")
        {
            return new ApiResponse<T>
            {
                status = (int)HttpStatusCode.BadRequest,
                Success = false,
                Message = message,
                Errors = errors,
            };
        }
    }
}

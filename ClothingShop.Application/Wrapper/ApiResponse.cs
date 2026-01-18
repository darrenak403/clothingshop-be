using System.Net;

namespace ClothingShop.Application.Wrapper
{

    public class ApiResponse<T>
    {
        public int Status { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public string? Errors { get; set; }
        public static ApiResponse<T> SuccessResponse(T data, string message = "Thành công", HttpStatusCode status = HttpStatusCode.OK)
        {
            return new ApiResponse<T>
            {
                Status = (int)status,
                Success = true,
                Message = message,
                Data = data,
            };
        }
        public static ApiResponse<T> FailureResponse(string errors, string message = "Thất bại", HttpStatusCode status = HttpStatusCode.BadRequest)
        {
            return new ApiResponse<T>
            {
                Status = (int)status,
                Success = false,
                Message = message,
                Errors = errors,
            };
        }
        public static ApiResponse<PagedResult<T>> SuccessPagedResponse(
            IEnumerable<T> items,
            int totalItems,
            int pageNumber,
            int pageSize,
            string message = "Thành công")
        {
            var pagedResult = new PagedResult<T>(items, pageNumber, pageSize, totalItems);
            return new ApiResponse<PagedResult<T>>
            {
                Status = (int)HttpStatusCode.OK,
                Success = true,
                Message = message,
                Data = pagedResult,
            };
        }
    }
}

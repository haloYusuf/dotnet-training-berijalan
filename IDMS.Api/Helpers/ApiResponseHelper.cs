using IDMS.Shared.Common;

namespace IDMS.Api.Helpers;

public static class ApiResponseHelper
{
    public static ApiResponse<T> Success<T>(HttpContext httpContext, T? data, string message = "Data retrieved successfully")
    {
        return new ApiResponse<T>
        {
            ReqId = httpContext.TraceIdentifier,
            Status = "success",
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Success<T>(HttpContext httpContext, T? data, int page, int limit, int totalItems, string message = "Data retrieved successfully")
    {
        var totalPages = limit > 0 ? (int)Math.Ceiling(totalItems / (double)limit) : 0;

        return new ApiResponse<T>
        {
            ReqId = httpContext.TraceIdentifier,
            Status = "success",
            Message = message,
            Data = data,
            Pagination = new Pagination
            {
                CurrentPage = page,
                Limit = limit,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasNextPage = page < totalPages,
                HasPreviousPage = page > 1
            }
        };
    }

    public static ApiResponse<T> Error<T>(HttpContext httpContext, string message, T? data = default)
    {
        return new ApiResponse<T>
        {
            ReqId = httpContext.TraceIdentifier,
            Status = "error",
            Message = message,
            Data = data
        };
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Success") =>
            new() { Success = true, Message = message, Data = data };

        public static ApiResponse<T> Fail(string message, List<string>? errors = null) =>
            new() { Success = false, Message = message, Errors = errors };
    }

    public class PagedResponse<T> : ApiResponse<List<T>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public static PagedResponse<T> Ok(List<T> data, int page, int pageSize, int totalCount) =>
            new()
            {
                Success = true,
                Message = "Success",
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
    }
}
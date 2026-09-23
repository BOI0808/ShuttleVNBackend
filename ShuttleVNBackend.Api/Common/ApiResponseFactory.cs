using ShuttleVNBackend.Api.DTOs.Common;

namespace ShuttleVNBackend.Api.Common;

public class ApiResponseFactory
{
    public static ApiResponse<T> Success<T>(T data)
    {
        return new ApiResponse<T>(true, data, null, null);
    }

    public static ApiResponse<T> Failure<T>(
        string message,
        IDictionary<string, string[]>? errors = null)
    {
        return new ApiResponse<T>(false, default, message, errors);
    }
}
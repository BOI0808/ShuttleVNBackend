namespace ShuttleVNBackend.Api.DTOs.Common;

public record ApiResponse<T>(
    bool Success,
    T? Data,
    string? Message,
    IDictionary<string, string[]>? Errors
);
using System.Net;

namespace ShuttleVNBackend.Application.Exceptions;

public class AppException : Exception
{
    protected AppException(string message, int statusCode, string code)
        : base(message)
    {
        StatusCode = statusCode;
        Code = code;
    }

    public int StatusCode { get; }
    public string Code { get; }
}

public class NotFoundException(string message = "Resource not found")
    : AppException(message, (int)HttpStatusCode.NotFound, "NOT_FOUND");

public class ValidationException(
    string message = "Validation failed",
    IDictionary<string, string[]>? errors = null)
    : AppException(message, (int)HttpStatusCode.BadRequest, "VALIDATION_ERROR")
{
    public IDictionary<string, string[]> Errors { get; } = errors ?? new Dictionary<string, string[]>();
}

public class UnauthorizedException(string message = "Unauthorized")
    : AppException(message, (int)HttpStatusCode.Unauthorized, "UNAUTHORIZED");

public class ForbiddenException(string message = "Forbidden")
    : AppException(message, (int)HttpStatusCode.Forbidden, "FORBIDDEN");

public class ConflictException(string message = "Resource already exists")
    : AppException(message, (int)HttpStatusCode.Conflict, "CONFLICT");
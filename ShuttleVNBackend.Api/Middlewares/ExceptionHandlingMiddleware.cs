using ShuttleVNBackend.Api.Common;
using ShuttleVNBackend.Application.Exceptions;
using ValidationException = ShuttleVNBackend.Application.Exceptions.ValidationException;

namespace ShuttleVNBackend.Api.Middlewares;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (AppException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex.StatusCode;

            IDictionary<string, string[]> errors = new Dictionary<string, string[]>();
            if (ex is ValidationException validationEx)
                errors = validationEx.Errors;

            var problem = ApiResponseFactory.Failure<object>(
                ex.Message, errors);

            await context.Response.WriteAsJsonAsync(problem);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(
                ApiResponseFactory.Failure<object>("Internal server error"));
        }
    }
}
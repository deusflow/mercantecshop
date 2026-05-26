using System.Net;
using System.Text.Json;
using WebShopMercantec.Exceptions;

namespace WebShopMercantec.Middleware;

// catches exceptions and returns standardized json errors
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // handle any unhandled exceptions in the pipe
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        // map custom exceptions to http status codes
        var (statusCode, message, errors) = exception switch
        {
            // 404 Not Found
            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                notFoundEx.Message,
                (Dictionary<string, string[]>?)null
            ),

            // 400 Bad Request
            BadRequestException badRequestEx => (
                HttpStatusCode.BadRequest,
                badRequestEx.Message,
                (Dictionary<string, string[]>?)null
            ),

            // 401 Unauthorized
            UnauthorizedException unauthorizedEx => (
                HttpStatusCode.Unauthorized,
                unauthorizedEx.Message,
                (Dictionary<string, string[]>?)null
            ),

            // 403 Forbidden
            ForbiddenException forbiddenEx => (
                HttpStatusCode.Forbidden,
                forbiddenEx.Message,
                (Dictionary<string, string[]>?)null
            ),

            // 402 Payment Required
            InsufficientCreditsException creditsEx => (
                HttpStatusCode.PaymentRequired,
                creditsEx.Message,
                new Dictionary<string, string[]>
                {
                    ["required"] = new[] { creditsEx.Required.ToString() },
                    ["available"] = new[] { creditsEx.Available.ToString() }
                }
            ),

            // 409 Conflict
            ProductNotAvailableException productEx => (
                HttpStatusCode.Conflict,
                productEx.Message,
                productEx.ProductId.HasValue
                    ? new Dictionary<string, string[]>
                    {
                        ["productId"] = new[] { productEx.ProductId.Value.ToString() },
                        ["reason"] = new[] { productEx.Reason ?? "Unknown" }
                    }
                    : null
            ),

            // 500 Internal Server Error fallback
            _ => (
                HttpStatusCode.InternalServerError,
                _env.IsDevelopment()
                    ? exception.Message  // show details in dev
                    : "An internal server error occurred",  // generic message for prod
                (Dictionary<string, string[]>?)null
            )
        };

        // build response object
        var response = new ErrorResponse
        {
            StatusCode = (int)statusCode,
            Message = message,
            Errors = errors,
            StackTrace = _env.IsDevelopment() ? exception.StackTrace : null,
            Timestamp = DateTime.UtcNow
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        // return serialized json
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _env.IsDevelopment() // pretty print only in dev
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}

// standard error response format
public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string[]>? Errors { get; set; }
    public string? StackTrace { get; set; }
    public DateTime Timestamp { get; set; }
}

public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}

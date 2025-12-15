using System.Net;
using System.Text.Json;
using WebShopMercantec.Exceptions;

namespace WebShopMercantec.Middleware;

/// <summary>
/// Middleware для глобальной обработки ошибок
/// Перехватывает все исключения и преобразует их в правильные HTTP ответы
/// 
/// ЧТО ДЕЛАЕТ:
/// 1. Перехватывает все исключения в приложении
/// 2. Определяет правильный HTTP Status Code
/// 3. Форматирует ответ в JSON
/// 4. Логирует ошибку
/// 5. Скрывает детали ошибок в production
/// 
/// КАК РАБОТАЕТ:
/// - Оборачивает весь pipeline в try-catch
/// - Если exception - обрабатывает и возвращает JSON с ошибкой
/// - Если всё ок - передает управление дальше
/// </summary>
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

    /// <summary>
    /// Главный метод middleware - вызывается для каждого HTTP запроса
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Передаем управление следующему middleware
            await _next(context);
        }
        catch (Exception ex)
        {
            // Если поймали исключение - обрабатываем
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Обработчик исключений - преобразует exception в HTTP ответ
    /// </summary>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Логируем ошибку
        _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

        // Определяем HTTP статус и сообщение в зависимости от типа исключения
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

            // 402 Payment Required (или 400)
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

            // 500 Internal Server Error (все остальные ошибки)
            _ => (
                HttpStatusCode.InternalServerError,
                _env.IsDevelopment()
                    ? exception.Message  // В dev показываем детали
                    : "An internal server error occurred",  // В prod - общее сообщение
                (Dictionary<string, string[]>?)null
            )
        };

        // Формируем JSON ответ
        var response = new ErrorResponse
        {
            StatusCode = (int)statusCode,
            Message = message,
            Errors = errors,
            // В dev режиме добавляем stack trace
            StackTrace = _env.IsDevelopment() ? exception.StackTrace : null,
            Timestamp = DateTime.UtcNow
        };

        // Настраиваем HTTP ответ
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        // Сериализуем и отправляем JSON
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _env.IsDevelopment() // Pretty print в dev
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}

/// <summary>
/// Модель для JSON ответа с ошибкой
/// 
/// ПРИМЕР ОТВЕТА:
/// {
///   "statusCode": 404,
///   "message": "Product with ID 123 not found",
///   "errors": null,
///   "stackTrace": null,
///   "timestamp": "2025-12-15T10:30:00Z"
/// }
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// HTTP Status Code (404, 400, 500 и т.д.)
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Сообщение об ошибке
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Дополнительные детали ошибки (опционально)
    /// Например, для валидации - список полей с ошибками
    /// </summary>
    public Dictionary<string, string[]>? Errors { get; set; }

    /// <summary>
    /// Stack trace (только в Development режиме)
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// Время когда произошла ошибка
    /// </summary>
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Extension method для регистрации middleware
/// Позволяет использовать app.UseErrorHandling() в Program.cs
/// </summary>
public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}


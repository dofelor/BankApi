using BankApi.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Infrastructure.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // Бизнес-ошибки — Info уровень, остальное — Error
            if (exception is BusinessRuleException or NotFoundException or ArgumentException)
                _logger.LogWarning(exception, "Business rule or validation error: {Message}", exception.Message);
            else
                _logger.LogError(exception, "Unhandled exception: {ExceptionType} — {Message}",
                    exception.GetType().Name, exception.Message);

            var (statusCode, title) = exception switch
            {
                // Кастомные бизнес-исключения
                BusinessRuleException   => (StatusCodes.Status422UnprocessableEntity, "Business Rule Violation"),
                NotFoundException       => (StatusCodes.Status404NotFound,            "Resource Not Found"),

                // Стандартные .NET исключения
                KeyNotFoundException    => (StatusCodes.Status404NotFound,            "Resource Not Found"),
                InvalidOperationException => (StatusCodes.Status422UnprocessableEntity, "Operation Not Allowed"),
                ArgumentException       => (StatusCodes.Status400BadRequest,          "Invalid Argument"),
                UnauthorizedAccessException => (StatusCodes.Status403Forbidden,       "Forbidden"),

                // Всё остальное — 500
                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
            };

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Status   = statusCode,
                Title    = title,
                Detail   = exception.Message,
                Instance = httpContext.Request.Path,
                Extensions =
                {
                    ["exceptionType"] = exception.GetType().Name
                }
            };

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
    }
}

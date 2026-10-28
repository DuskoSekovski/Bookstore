using Microsoft.AspNetCore.Diagnostics;

namespace Bookstore.Api.Infrastructure
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is FluentValidation.ValidationException)
            {
                return false;
            }

            logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";

            var response = new
            {
                StatusCode = 500,
                Message = "An unexpected error occurred. Please try again later.",
                TraceId = httpContext.TraceIdentifier
            };

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            return true;
        }
    }
}

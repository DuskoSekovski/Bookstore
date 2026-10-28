using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Api.Infrastructure
{
    public class ValidationExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<ValidationExceptionHandler> _logger;

        public ValidationExceptionHandler(ILogger<ValidationExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if(exception is not ValidationException validationException)
            {
                return false;
            }

            _logger.LogWarning(exception, "A validation failure occurred during the request.");

            var errors = validationException.Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(failureGroup => failureGroup.Key,
                failureGroup => failureGroup.Select(f => f.ErrorMessage).ToArray());

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "about:blank",
                Title = "Validation Regimen Failed",
                Detail = "One or more incoming fields violated system property constraints.",
                Instance = httpContext.Request.Path
            };

            problemDetails.Extensions.Add("errors", errors);

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;

        }
    }
}

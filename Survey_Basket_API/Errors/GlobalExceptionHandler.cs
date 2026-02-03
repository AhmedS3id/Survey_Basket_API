using Microsoft.AspNetCore.Diagnostics;
using static System.Net.WebRequestMethods;

namespace Survey_Basket_API.Errors
{
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        private readonly ILogger _logger = logger;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Something went wrong {message}",exception.Message);
            var ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                Title = "Internal Server Error"
            };
            httpContext.Response.StatusCode=StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(ProblemDetails, cancellationToken: cancellationToken);
            return true;
        }
    }
}

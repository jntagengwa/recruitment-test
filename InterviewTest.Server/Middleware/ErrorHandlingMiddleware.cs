using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace InterviewTest.Server.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                _logger.LogInformation("Request {Method} {Path} responded {StatusCode}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = MediaTypeNames.Application.Json;

                var errorResponse = new
                {
                    status = context.Response.StatusCode,
                    message = "An unexpected error occurred. Please try again later.",
                    timestamp = DateTime.UtcNow
                };

                var result = JsonSerializer.Serialize(errorResponse);
                await context.Response.WriteAsync(result);
            }
        }
    }
}
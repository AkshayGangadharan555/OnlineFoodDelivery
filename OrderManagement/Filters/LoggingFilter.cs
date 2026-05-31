using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace Orders.Filters
{
    /// <summary>
    /// Logging filter that logs incoming HTTP requests and outgoing responses.
    /// Includes request/response timing for performance monitoring.
    /// </summary>
    public class LoggingFilter : IActionFilter
    {
        private readonly ILogger<LoggingFilter> _logger;
        private Stopwatch _stopwatch;

        public LoggingFilter(ILogger<LoggingFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch = Stopwatch.StartNew();

            var request = context.HttpContext.Request;
            var userIdentity = context.HttpContext.User?.Identity?.Name ?? "Anonymous";

            _logger.LogInformation(
                "Incoming Request: {Method} {Path} | User: {User} | Query: {QueryString}",
                request.Method,
                request.Path,
                userIdentity,
                request.QueryString
            );

            if (context.ActionArguments.Count > 0)
            {
                _logger.LogDebug("Request Arguments: {@Arguments}",
                    context.ActionArguments.Where(x => !IsPasswordField(x.Key))
                );
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch.Stop();

            var response = context.HttpContext.Response;
            var userIdentity = context.HttpContext.User?.Identity?.Name ?? "Anonymous";

            _logger.LogInformation(
                "Outgoing Response: {StatusCode} | User: {User} | Duration: {DurationMs}ms",
                response.StatusCode,
                userIdentity,
                _stopwatch.ElapsedMilliseconds
            );

            if (context.Exception != null && !context.ExceptionHandled)
            {
                _logger.LogError(context.Exception,
                    "Exception during action execution: {ExceptionMessage}",
                    context.Exception.Message
                );
            }
        }

        private bool IsPasswordField(string fieldName)
        {
            return fieldName.Contains("password", StringComparison.OrdinalIgnoreCase) ||
                   fieldName.Contains("secret", StringComparison.OrdinalIgnoreCase) ||
                   fieldName.Contains("token", StringComparison.OrdinalIgnoreCase);
        }
    }
}

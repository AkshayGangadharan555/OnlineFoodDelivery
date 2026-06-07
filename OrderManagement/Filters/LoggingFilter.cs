using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace Orders.Filters
{
    /// <summary>
    /// Lightweight logging filter for request/response timing and basic logging.
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
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch.Stop();
            var statusCode = context.HttpContext.Response.StatusCode;
            var method = context.HttpContext.Request.Method;
            var path = context.HttpContext.Request.Path;
            var duration = _stopwatch.ElapsedMilliseconds;

            if (statusCode >= 400)
            {
                _logger.LogWarning("{Method} {Path} - {StatusCode} ({DurationMs}ms)", method, path, statusCode, duration);
            }
        }
    }
}

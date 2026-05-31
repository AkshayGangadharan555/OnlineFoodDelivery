using Microsoft.AspNetCore.Mvc.Filters;

namespace Orders.Filters
{
    /// <summary>
    /// Authorization logging filter that logs authorization events.
    /// Useful for security auditing and debugging authorization issues.
    /// </summary>
    public class AuthorizationLoggingFilter : IAuthorizationFilter
    {
        private readonly ILogger<AuthorizationLoggingFilter> _logger;

        public AuthorizationLoggingFilter(ILogger<AuthorizationLoggingFilter> logger)
        {
            _logger = logger;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            var isAuthenticated = user?.Identity?.IsAuthenticated ?? false;
            var userName = user?.Identity?.Name ?? "Anonymous";
            var endpoint = context.HttpContext.Request.Path;
            var method = context.HttpContext.Request.Method;

            if (isAuthenticated)
            {
                _logger.LogInformation(
                    "Authorization Check: User {UserName} is attempting {Method} {Endpoint}",
                    userName, method, endpoint
                );

                var userClaims = user?.Claims?.Select(c => new { c.Type, c.Value }).ToList();
                _logger.LogDebug("User Claims: {@Claims}", userClaims);
            }
            else
            {
                _logger.LogWarning(
                    "Unauthenticated access attempt to {Method} {Endpoint}",
                    method, endpoint
                );
            }
        }
    }
}

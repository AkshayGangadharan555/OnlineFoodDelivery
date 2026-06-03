using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Orders.DTOs.ResponseDTOs;

namespace Orders.Filters
{
    /// <summary>
    /// Global exception handling filter for unhandled exceptions.
    /// </summary>
    public class ExceptionHandlingFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionHandlingFilter> _logger;

        public ExceptionHandlingFilter(ILogger<ExceptionHandlingFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Unhandled exception in {Action}",
                context.ActionDescriptor.DisplayName);

            var response = new ApiResponse<object>(
                status: StatusCodes.Status500InternalServerError,
                message: "An internal server error occurred.",
                data: null
            );

            context.Result = new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            context.ExceptionHandled = true;
        }
    }
}

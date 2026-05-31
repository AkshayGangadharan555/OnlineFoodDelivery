using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Orders.DTOs.ResponseDTOs;

namespace Orders.Filters
{
    /// <summary>
    /// Global exception handling filter that catches unhandled exceptions
    /// and returns standardized error responses.
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
            _logger.LogError(context.Exception, "Unhandled exception occurred in {ActionName}. Exception: {ExceptionMessage}",
                context.ActionDescriptor.DisplayName, context.Exception.Message);

            var response = new ApiResponse<object>(
                status: StatusCodes.Status500InternalServerError,
                message: "An internal server error occurred. Please try again later.",
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

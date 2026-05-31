using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Orders.DTOs.ResponseDTOs;

namespace Orders.Filters
{
    /// <summary>
    /// Validation filter that checks ModelState and returns validation errors
    /// in a standardized format.
    /// </summary>
    public class ValidationFilter : IActionFilter
    {
        private readonly ILogger<ValidationFilter> _logger;

        public ValidationFilter(ILogger<ValidationFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(ms => ms.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToList() ?? new List<string>()
                    );

                _logger.LogWarning("Model validation failed for {ActionName}. Errors: {@Errors}",
                    context.ActionDescriptor.DisplayName, errors);

                var response = new ApiResponse<object>(
                    status: StatusCodes.Status400BadRequest,
                    message: "Validation failed",
                    data: errors
                );

                context.Result = new BadRequestObjectResult(response);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No action needed after execution for validation filter
        }
    }
}

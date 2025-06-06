using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace TendersAPI.WebApi.Filters
{
    public class LogBadRequestAndStatusCodeFilter : IActionFilter
    {
        private readonly ILogger<LogBadRequestAndStatusCodeFilter> _logger;

        public LogBadRequestAndStatusCodeFilter(ILogger<LogBadRequestAndStatusCodeFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is BadRequestResult || context.Result is BadRequestObjectResult)
            {
                _logger.LogWarning("Action returned BadRequest: {ActionName}", context.ActionDescriptor.DisplayName);
            }
            else if (context.Result is ObjectResult objectResult)
            {
                if (objectResult.StatusCode.HasValue && objectResult.StatusCode.Value > 0)
                {
                    _logger.LogInformation("Action returned status code {StatusCode}: {ActionName}", objectResult.StatusCode, context.ActionDescriptor.DisplayName);
                }
            }
            else if (context.Result is StatusCodeResult statusCodeResult)
            {
                if (statusCodeResult.StatusCode > 0)
                {
                    _logger.LogInformation("Action returned status code {StatusCode}: {ActionName}", statusCodeResult.StatusCode, context.ActionDescriptor.DisplayName);
                }
            }
        }
    }
}

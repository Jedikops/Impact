namespace TendersAPI.WebApi.Middlewares
{
    public class BadRequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<BadRequestLoggingMiddleware> _logger;

        public BadRequestLoggingMiddleware(RequestDelegate next, ILogger<BadRequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 400)
            {
                // Log details about the 400 response
                var method = context.Request.Method;
                var path = context.Request.Path;
                var query = context.Request.QueryString.Value;
                _logger.LogWarning("400 Bad Request detected: {Method} {Path}{Query}", method, path, query);

            }
        }
    }
}

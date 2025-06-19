namespace TourManagementApi.Middleware
{
    public class RequestHeaderValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestHeaderValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey("X-REQUEST-IDENTIFIER"))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Missing X-REQUEST-IDENTIFIER header");
                return;
            }

            if (!context.Request.Headers["Accept"].ToString().Contains("application/json"))
            {
                context.Response.StatusCode = 406;
                await context.Response.WriteAsync("Accept header must include application/json");
                return;
            }

            if (!context.Request.ContentType?.Contains("application/json") ?? true)
            {
                context.Response.StatusCode = 415;
                await context.Response.WriteAsync("Content-Type must be application/json");
                return;
            }

            await _next(context);
        }
    }

}

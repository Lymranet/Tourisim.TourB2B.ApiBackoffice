namespace TourManagementApi.Middleware
{
    public class RequestHeaderValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;

        public RequestHeaderValidationMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Development ortamındaysa ve localhost'tan gelen istekse, kontrol etme
            if (_env.IsDevelopment() && context.Request.Host.Host == "localhost")
            {
                await _next(context);
                return;
            }

            // Sadece API isteklerinde kontrol yap (JSON içerikli)
            var isApiRequest = context.Request.Path.StartsWithSegments("/api")
                               || context.Request.Headers["Accept"].ToString().Contains("application/json");

            if (isApiRequest)
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
            }

            await _next(context);
        }
    }
}

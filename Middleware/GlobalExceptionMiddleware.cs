using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace TourManagementApi.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            string correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault() ?? Guid.NewGuid().ToString();
            context.Response.Headers["X-Correlation-Id"] = correlationId;

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Exception loglama (CorrelationId ve Path ile birlikte)
                _logger.LogError(ex, "Bir sistem hatası oluştu. Path: {Path}, CorrelationId: {CorrelationId}", context.Request.Path, correlationId);

                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                // API istekleri için JSON response dön
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.ContentType = "application/json";
                    var problem = new
                    {
                        error = "Internal server error",
                        detail = ex.Message,
                        correlationId = correlationId
                    };
                    await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
                }
                else
                {
                    // MVC istekleri için Error view dön
                    context.Response.ContentType = "text/html";

                    var routeData = new RouteData();
                    routeData.Values["controller"] = "Home";
                    routeData.Values["action"] = "Error";

                    var actionContext = new ActionContext(context, routeData, new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

                    var result = new ViewResult
                    {
                        ViewName = "~/Views/Shared/Error.cshtml",
                        ViewData = new ViewDataDictionary<Exception>(
                            new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                            new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary())
                        {
                            Model = ex
                        }
                    };

                    await result.ExecuteResultAsync(actionContext);
                }
            }
        }
    }
}

using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;

namespace TourManagementApi.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly string _logDirectory = "Logs";

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;

            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }

        public async Task Invoke(HttpContext context)
        {
            string dateSuffix = DateTime.UtcNow.ToString("yyyy-MM-dd");
            string requestLogPath = Path.Combine(_logDirectory, $"requests-{dateSuffix}.log");
            string responseLogPath = Path.Combine(_logDirectory, $"responses-{dateSuffix}.log");

            // -----------------------
            // Request Logging
            // -----------------------
            context.Request.EnableBuffering();
            string requestBody = "";

            if (context.Request.ContentLength > 0 && context.Request.ContentType?.Contains("application/json") == true)
            {
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            var requestLog = new StringBuilder();
            requestLog.AppendLine("===== [HTTP REQUEST] =====");
            requestLog.AppendLine($"Timestamp: {DateTime.UtcNow:O}");
            requestLog.AppendLine($"Method: {context.Request.Method}");
            requestLog.AppendLine($"Path: {context.Request.Path}");
            requestLog.AppendLine($"Full URL: {context.Request.GetDisplayUrl()}");
            requestLog.AppendLine($"Query: {context.Request.QueryString}");
            requestLog.AppendLine($"IP Address: {context.Connection.RemoteIpAddress}");
            requestLog.AppendLine($"User-Agent: {context.Request.Headers["User-Agent"]}");
            requestLog.AppendLine($"Content-Type: {context.Request.ContentType}");
            requestLog.AppendLine($"Accept: {context.Request.Headers["Accept"]}");
            requestLog.AppendLine("Headers:");
            foreach (var header in context.Request.Headers)
            {
                requestLog.AppendLine($"{header.Key}: {header.Value}");
            }
            requestLog.AppendLine("Body:");
            requestLog.AppendLine(requestBody);
            requestLog.AppendLine("==========================\n");

            await File.AppendAllTextAsync(requestLogPath, requestLog.ToString());

            // -----------------------
            // Response Logging
            // -----------------------
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            var responseLog = new StringBuilder();

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception caught in RequestResponseLoggingMiddleware");
                responseLog.AppendLine("***** EXCEPTION *****");
                responseLog.AppendLine($"Timestamp: {DateTime.UtcNow:O}");
                responseLog.AppendLine($"Message: {ex.Message}");
                responseLog.AppendLine($"StackTrace: {ex.StackTrace}");
                responseLog.AppendLine("*********************\n");
                throw; // yeniden fırlat
            }

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var endpoint = context.GetEndpoint();
            var endpointDisplayName = endpoint?.DisplayName ?? "Unknown Endpoint";
            responseLog.AppendLine("===== [HTTP RESPONSE] =====");
            responseLog.AppendLine($"Handled By: {endpointDisplayName}");
            responseLog.AppendLine($"Timestamp: {DateTime.UtcNow:O}");
            responseLog.AppendLine($"Status Code: {context.Response.StatusCode}");
            responseLog.AppendLine("Headers:");
            foreach (var header in context.Response.Headers)
            {
                responseLog.AppendLine($"{header.Key}: {header.Value}");
            }
            responseLog.AppendLine("Body:");
            responseLog.AppendLine(responseText);
            responseLog.AppendLine("===========================\n");

            await File.AppendAllTextAsync(responseLogPath, responseLog.ToString());

            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}

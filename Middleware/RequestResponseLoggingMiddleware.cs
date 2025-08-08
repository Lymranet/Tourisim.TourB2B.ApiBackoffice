using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Serilog.Context;
using System.Diagnostics;
using System.Text;

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
        }
        private async Task SafeAppendToFileAsync(string filePath, string content)
        {
            byte[] data = Encoding.UTF8.GetBytes(content);
            using var stream = new FileStream(
                filePath,
                FileMode.Append,
                FileAccess.Write,
                FileShare.ReadWrite, // ⬅️ başka işlemler de okuyabilsin/yazabilsin
                4096,
                useAsync: true
            );
            await stream.WriteAsync(data, 0, data.Length);
        }

        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var routePath = context.Request.Path.Value?.ToLower() ?? string.Empty;

            string requestType = "UNKNOWN";
            if (routePath.StartsWith("/api") || endpoint?.Metadata?.GetMetadata<ApiControllerAttribute>() != null)
            {
                requestType = "API";
            }
            else if (endpoint?.Metadata?.GetMetadata<ControllerAttribute>() != null)
            {
                requestType = "MVC";
            }

            LogContext.PushProperty("RequestType", requestType);

            var today = DateTime.UtcNow;
            var logDir = Path.Combine(_logDirectory, today.ToString("yyyy"), today.ToString("MM"), today.ToString("dd"), requestType);
            Directory.CreateDirectory(logDir);

            string requestLogPath = Path.Combine(logDir, "requests.log");
            string responseLogPath = Path.Combine(logDir, "responses.log");
            string errorLogPath = Path.Combine(logDir, "errors.log");

            context.Request.EnableBuffering();
            string requestBody = string.Empty;

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
            requestLog.AppendLine($"Controller: {context.GetRouteData()?.Values["controller"]}");
            requestLog.AppendLine($"Action: {context.GetRouteData()?.Values["action"]}");
            requestLog.AppendLine($"RequestType: {requestType}");
            requestLog.AppendLine("Headers:");
            foreach (var header in context.Request.Headers)
            {
                requestLog.AppendLine($"{header.Key}: {header.Value}");
            }
            requestLog.AppendLine("Body:");
            requestLog.AppendLine(requestBody);
            requestLog.AppendLine("==========================\n");

            await SafeAppendToFileAsync(requestLogPath, requestLog.ToString());

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
                var trace = new StackTrace(ex, true);
                var frame = trace.GetFrames()?.FirstOrDefault(f => f.GetFileLineNumber() > 0);
                var fileName = frame?.GetFileName();
                var line = frame?.GetFileLineNumber();
                var method = frame?.GetMethod()?.Name;

                var errorDetails = new StringBuilder();
                errorDetails.AppendLine("***** EXCEPTION *****");
                errorDetails.AppendLine($"Timestamp: {DateTime.UtcNow:O}");
                errorDetails.AppendLine($"Message: {ex.Message}");
                errorDetails.AppendLine($"Method: {method}");
                errorDetails.AppendLine($"File: {fileName}");
                errorDetails.AppendLine($"Line: {line}");
                errorDetails.AppendLine($"StackTrace: {ex.StackTrace}");
                errorDetails.AppendLine("*********************\n");

                await SafeAppendToFileAsync(errorLogPath, errorDetails.ToString());
                _logger.LogError(ex, "Exception caught in RequestResponseLoggingMiddleware");
                throw;
            }

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            responseLog.AppendLine("===== [HTTP RESPONSE] =====");
            responseLog.AppendLine($"Handled By: {endpoint?.DisplayName ?? "Unknown Endpoint"}");
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

            await SafeAppendToFileAsync(responseLogPath, responseLog.ToString());

            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}
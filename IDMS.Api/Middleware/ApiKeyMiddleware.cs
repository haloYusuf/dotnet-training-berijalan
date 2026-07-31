using System.Net;
using System.Text.Json;
using IDMS.Shared.Common;

namespace IDMS.Api.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string API_KEY_HEADER = "X-Api-Key";

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(API_KEY_HEADER, out var extractedApiKey))
            {
                await WriteUnauthorizedResponse(context, "Unauthorized");
                return;
            }

            var apiKey = context.RequestServices.GetRequiredService<IConfiguration>()
            .GetValue<string>("ApiKey");
            if (string.IsNullOrEmpty(apiKey) || !apiKey.Equals(extractedApiKey))
            {
                await WriteUnauthorizedResponse(context, "Unauthorized");
                return;
            }

            await _next(context);
        }

        private static async Task WriteUnauthorizedResponse(HttpContext context, string message)
        {
            var response = new ApiResponse<object>
            {
                ReqId = context.TraceIdentifier,
                Status = "error",
                Message = message,
                Data = null
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }

    }
}
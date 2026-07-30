using System.Net;
using System.Text.Json;

namespace IDMS.Web.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedAccessException)
            {
                if (IsApiRequest(context))
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    await WriteError(context, 401, "Unauthorized");
                }
                else
                {
                    context.Response.Redirect("/Auth/Login");
                }
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning("Not found: {Message}", ex.Message);
                if (IsApiRequest(context))
                {
                    context.Response.StatusCode = 404;
                    context.Response.ContentType = "application/json";
                    await WriteError(context, 404, ex.Message);
                }
                else
                {
                    context.Response.Redirect($"/Home/Error?message={Uri.EscapeDataString(ex.Message)}");
                }
            }
            catch (BadRequestException ex)
            {
                _logger.LogWarning("Bad request: {Message}", ex.Message);
                if (IsApiRequest(context))
                {
                    context.Response.StatusCode = 400;
                    context.Response.ContentType = "application/json";
                    await WriteError(context, 400, ex.Message);
                }
                else
                {
                    context.Response.Redirect($"/Home/Error?message={Uri.EscapeDataString(ex.Message)}");
                }
            }
            catch (ApiException ex)
            {
                _logger.LogWarning("API error ({Status}): {Message}", ex.StatusCode, ex.Message);
                if (IsApiRequest(context))
                {
                    context.Response.StatusCode = ex.StatusCode;
                    context.Response.ContentType = "application/json";
                    await WriteError(context, ex.StatusCode, ex.Message);
                }
                else
                {
                    context.Response.Redirect($"/Home/Error?message={Uri.EscapeDataString(ex.Message)}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");
                if (IsApiRequest(context))
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    await WriteError(context, 500, "An internal server error occurred. Please try again later.");
                }
                else
                {
                    context.Response.Redirect("/Home/Error");
                }
            }
        }

        private static bool IsApiRequest(HttpContext context)
        {
            return context.Request.Headers.Accept.ToString().Contains("application/json")
                || context.Request.Headers.XRequestedWith == "XMLHttpRequest"
                || context.Request.Path.StartsWithSegments("/Brand/List")
                || context.Request.Path.StartsWithSegments("/Brand/Detail")
                || context.Request.Path.StartsWithSegments("/Brand/Create")
                || context.Request.Path.StartsWithSegments("/Brand/Update")
                || context.Request.Path.StartsWithSegments("/Brand/Delete");
        }

        private static async Task WriteError(HttpContext context, int statusCode, string message)
        {
            var response = new
            {
                status = "Error",
                message,
                data = (object?)null,
                pagination = (object?)null
            };
            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            await context.Response.WriteAsync(json);
        }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }
    }

    public class ApiException : Exception
    {
        public int StatusCode { get; }
        public ApiException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}

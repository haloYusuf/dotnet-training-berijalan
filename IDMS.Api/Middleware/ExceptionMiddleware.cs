using System.Net;
using System.Text.Json;
using IDMS.Shared.Common;
using IDMS.Shared.Exceptions;

namespace IDMS.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            if (ex is NotFoundException or BadRequestException or ConflictException)
                _logger.LogWarning("Validation error: {Message}", ex.Message);
            else
                _logger.LogError(ex, "Unhandled exception");

            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            NotFoundException => HttpStatusCode.NotFound,
            BadRequestException => HttpStatusCode.BadRequest,
            ConflictException => HttpStatusCode.Conflict,
            _ => HttpStatusCode.InternalServerError
        };

        var message = exception switch
        {
            NotFoundException or BadRequestException or ConflictException => exception.Message,
            _ => "An internal server error occurred"
        };

        var response = new ApiResponse<object>
        {
            ReqId = context.TraceIdentifier,
            Status = "error",
            Message = message,
            Data = null
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

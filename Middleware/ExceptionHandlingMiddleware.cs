using System.Text;
using System.Text.Json;
using TreeAPI.Data;
using TreeAPI.Exceptions;
using TreeAPI.Models;

namespace TreeAPI.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        context.Request.EnableBuffering();

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred");

            await HandleExceptionAsync(context, ex, dbContext);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, ApplicationDbContext dbContext)
    {
        string eventId = DateTime.UtcNow.Ticks.ToString();

        try
        {
            context.Request.Body.Position = 0;

            string bodyString;
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                bodyString = await reader.ReadToEndAsync();
            }

            string queryString = context.Request.QueryString.ToString();

            var logEntry = new ExceptionLog
            {
                EventId = eventId,
                Timestamp = DateTime.UtcNow,
                QueryParameters = queryString,
                BodyParameters = bodyString,
                ExceptionType = exception.GetType().Name,
                ExceptionMessage = exception.Message,
                StackTrace = exception.StackTrace ?? string.Empty
            };

            if (exception is SecureException secureException)
            {
                eventId = secureException.EventId;
                logEntry.EventId = eventId;
            }

            try
            {
                await dbContext.ExceptionLogs.AddAsync(logEntry);
                await dbContext.SaveChangesAsync();
                _logger.LogInformation($"Exception logged with ID: {eventId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log exception to database");
            }

            if (!context.Response.HasStarted)
            {
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                ErrorResponse response;

                if (exception is SecureException)
                {
                    response = new ErrorResponse
                    {
                        Type = "Secure",
                        Id = eventId,
                        Data = new ErrorData
                        {
                            Message = exception.Message
                        }
                    };
                }
                else
                {
                    response = new ErrorResponse
                    {
                        Type = "Exception",
                        Id = eventId,
                        Data = new ErrorData
                        {
                            Message = $"Internal server error ID = {eventId}"
                        }
                    };
                }

                var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                _logger.LogInformation($"Sending error response: {jsonResponse}");
                await context.Response.WriteAsync(jsonResponse);
            }
            else
            {
                _logger.LogWarning("Response has already started, cannot write exception response");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling exception");
            
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync($"{{\"type\":\"Exception\",\"id\":\"{eventId}\",\"data\":{{\"message\":\"Internal server error\"}}}}");
            }
        }
    }
}
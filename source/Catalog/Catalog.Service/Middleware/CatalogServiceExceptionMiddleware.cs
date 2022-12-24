using System.Net;
using Catalog.Service.Exceptions;

namespace Catalog.Service.Middleware;

public sealed class CatalogServiceExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CatalogServiceExceptionMiddleware> _logger;

    public CatalogServiceExceptionMiddleware(RequestDelegate next, ILogger<CatalogServiceExceptionMiddleware> logger)
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
            _logger.LogError($"Something went wrong: {ex}");
            await HandleExceptionAsync(httpContext, ex);
        }
    }
    
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/text";
        context.Response.StatusCode = exception switch
        {
            ProductNotFoundException productNotFoundException => (int) HttpStatusCode.NotFound,
            _ => (int) HttpStatusCode.InternalServerError
        };
        await context.Response.WriteAsync(new ErrorDetails(context.Response.StatusCode, exception.Message).ToString());
    }
}

public sealed class ErrorDetails
{
    private const string ERRORDETAIL_RESPONSE = "Responded with StatusCode: {0} with Message: {1}";
    public int StatusCode { get; }
    public string Message { get; }

    public ErrorDetails(int statusCode, string message)
    {
        StatusCode = statusCode;
        Message = message;
    }

    public override string ToString()
        => string.Format(ERRORDETAIL_RESPONSE, StatusCode, Message);
}
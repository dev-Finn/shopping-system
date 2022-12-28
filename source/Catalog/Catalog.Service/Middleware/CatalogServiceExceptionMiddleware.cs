using System.Net;
using Catalog.Service.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            _logger.LogError(ex, "Something went wrong");
            await HandleExceptionAsync(httpContext, ex);
        }
    }
    
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        ICatalogProblemDetails problemDetails = exception switch
        {
            ProductNotFoundException productNotFoundException => productNotFoundException.AsProblemDetails(),
            PleaseRetryAgainException pleaseRetryAgainException => pleaseRetryAgainException.AsProblemDetails(),
            CommitFailedException commitFailedException => commitFailedException.AsProblemDetails(),
            _ => new UnexpectedDetail(context.Request.QueryString.Value)
        };
        
        context.Response.ContentType = problemDetails.ContentType;
        context.Response.StatusCode = problemDetails.StatusCode;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}

public sealed class UnexpectedDetail : ProblemDetails, ICatalogProblemDetails
{
    private const string TYPE_TEXT = "UNEXPECTED_FAULT";
    private const string TITLE_TEXT = "Unexpected Service Behavior";
    private const string DETAIL_TEXT = "Hoppla, something unexpected happened. Please try again later.";
    public int StatusCode => 500;
    public string ContentType => "application/json";

    public UnexpectedDetail(string? instance)
    {
        Type = TYPE_TEXT;
        Title = TITLE_TEXT;
        Detail = DETAIL_TEXT;
        Instance = instance;
    }
}
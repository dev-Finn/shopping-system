using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Service.Exceptions;

[Serializable]
public class PleaseRetryAgainException : Exception
{
    public PleaseRetryAgainException(string message, Exception innerEx) : base(message, innerEx)
    {
    }

    private PleaseRetryAgainException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        
    }
    
    public PleaseTryAgainProblemDetails AsProblemDetails() => new();
}

public sealed class PleaseTryAgainProblemDetails : ProblemDetails, ICatalogProblemDetails
{
    public int StatusCode => 500;
    public string ContentType => "application/json";
}
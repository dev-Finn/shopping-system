using System.Runtime.Serialization;

namespace Catalog.Service.Exceptions;

[Serializable]
public class CommitFailedException : Exception
{
    public CommitFailedException(string message, Exception innerEx) : base(message, innerEx)
    {
    }

    private CommitFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        
    }
    
    public CommitFailedProblemDetails AsProblemDetails() => new();
}

public sealed class CommitFailedProblemDetails : ICatalogProblemDetails
{
    public int StatusCode => 500;
    public string ContentType => "application/json";
}
namespace Catalog.Service.Exceptions;

public interface ICatalogProblemDetails
{
    int StatusCode { get; }
    string ContentType { get; }
}
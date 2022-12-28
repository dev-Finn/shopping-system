using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Service.Exceptions;

[Serializable]
public class ProductNotFoundException : Exception
{
    public Guid ProductId { get; }
    public ProductNotFoundException(Guid productId, string message) : base(message)
    {
        ProductId = productId;
    }

    private ProductNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        
    }

    public ProductNotFoundDetails AsProblemDetails() => new(ProductId);
}

public sealed class ProductNotFoundDetails : ProblemDetails, ICatalogProblemDetails
{
    private const string TYPE_TEXT = "RESOURCE_NOT_FOUND";
    private const string TITLE_TEXT = "Product not found";
    private const string DETAIL_TEXT = "Requested Product with Id='{0}' not found.";
    private const string INSTANCE_TEXT = "/products/{0}";
    public int StatusCode => 404;
    public string ContentType => "application/json";

    public ProductNotFoundDetails(Guid notFound)
    {
        Type = TYPE_TEXT;
        Title = TITLE_TEXT;
        Detail = string.Format(DETAIL_TEXT, notFound);
        Instance = string.Format(INSTANCE_TEXT, notFound);
    }
}
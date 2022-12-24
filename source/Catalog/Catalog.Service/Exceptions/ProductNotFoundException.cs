using System.Runtime.Serialization;

namespace Catalog.Service.Exceptions;

[Serializable]
public class ProductNotFoundException : Exception
{
    public ProductNotFoundException(string message) : base(message)
    {
    }

    private ProductNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        
    }
}
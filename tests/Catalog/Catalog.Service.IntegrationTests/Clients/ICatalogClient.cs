using Newtonsoft.Json;
using Refit;

namespace Catalog.Service.IntegrationTests.Clients;

public sealed record CreateProductRequest(string Name, string Description , decimal Price);
public sealed record UpdateProductRequest(string Name, string Description, decimal Price);
public sealed record SieveModel(int Page, int PageSize);
public sealed class ProductDto
{
    [JsonProperty(propertyName: "id")]
    public Guid Id { get; set; }
    [JsonProperty(propertyName: "name")]
    public string Name { get; set; } 
    [JsonProperty(propertyName: "description")]
    public string Description { get; set; }
    [JsonProperty(propertyName: "price")]
    public decimal Price { get; set; }
    [JsonProperty(propertyName: "deletedAt")]
    public DateTime? DeletedAt { get; set; }
}
public sealed class GetPaginatedProductsResponse
{
    [JsonProperty(PropertyName = "data")]
    public List<ProductDto> Data { get; set; }
    
    [JsonProperty(PropertyName = "currentPage")]
    public int CurrentPage { get; init; }
    
    [JsonProperty(PropertyName = "pageSize")]
    public int PageSize { get; init; }

    [JsonProperty(PropertyName = "totalPages")]
    public int TotalPages { get; init; }
    
    [JsonProperty(PropertyName = "hasPrevious")]
    public bool HasPrevious { get; init; }
    
    [JsonProperty(PropertyName = "hasNext")]
    public bool HasNext { get; init; }
    
    [JsonProperty(PropertyName = "totalCount")]
    public int TotalCount { get; init; }
}

public interface ICatalogClient
{
    [Post("/products")]
    Task<IApiResponse> CreateProduct([Body] CreateProductRequest request);

    [Delete("/products/{productId}")]
    Task<IApiResponse> DeleteProduct(Guid productId);
    
    [Put("/products/{productId}")]
    Task<IApiResponse> UpdateProduct(Guid productId, [Body] UpdateProductRequest request);
    
    [Get("/products")]
    Task<ApiResponse<GetPaginatedProductsResponse>> GetPaginatedProducts(SieveModel request);
    
    [Get("/products/{productId}")]
    Task<ApiResponse<ProductDto>> GetProduct(Guid productId);
}
using Newtonsoft.Json;

namespace Shared.Contracts;

public abstract class PagedResponse<T>
{
    [JsonProperty(PropertyName = "data")]
    public IEnumerable<T> Data { get; init; }
    
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

    public PagedResponse(PaginatedList<T> data)
    {
        Data = data.ToList();
        CurrentPage = data.CurrentPage;
        PageSize = data.PageSize;
        TotalCount = data.TotalCount;
        TotalPages = data.TotalPages;
        HasNext = data.HasNext;
        HasPrevious = data.HasPrevious;
    }

    [JsonConstructor]
    public PagedResponse()
    {
    }
}
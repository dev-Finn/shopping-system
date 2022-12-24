namespace Catalog.Service.Helpers;

public sealed class PaginatedList<T> : List<T>
{
    public int CurrentPage { get; }
    public int TotalPages { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public PaginatedList(IEnumerable<T> items, int count, int page, int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = page;
        TotalPages = (int) Math.Ceiling(count / (double) pageSize);
        AddRange(items);
    }
}
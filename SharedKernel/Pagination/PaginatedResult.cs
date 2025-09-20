namespace SharedKernel.Pagination;

public sealed class PaginatedResult<TEntity>(
    int pageIndex, int pageSize, long count, IEnumerable<TEntity> data)
    where TEntity : class
{
    public int PageIndex { get; } = pageIndex;
    public int PageSize { get; } = pageSize;
    public long Count { get; } = count;
    public IEnumerable<TEntity> Data { get; } = data;
    public bool HasNextPage => PageIndex * PageSize < Count;
    public bool HasPreviousPage => PageIndex > 0;
    public int TotalPages => (int)Math.Ceiling((double)Count / PageSize);
}
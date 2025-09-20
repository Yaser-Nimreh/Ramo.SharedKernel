namespace SharedKernel.Pagination;

public sealed class CursorPaginatedResult<TEntity, T>(
    IEnumerable<TEntity> data, T? nextCursor, bool hasNextPage)
    where TEntity : class
{
    public IEnumerable<TEntity> Data { get; } = data;
    public T? NextCursor { get; } = nextCursor;
    public bool HasNextPage { get; } = hasNextPage;
}
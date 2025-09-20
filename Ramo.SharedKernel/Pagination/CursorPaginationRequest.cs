namespace SharedKernel.Pagination;

public sealed record CursorPaginationRequest<T>(T? Cursor, int PageSize = 10);
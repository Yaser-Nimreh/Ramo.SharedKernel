namespace SharedKernel.Pagination;

public sealed record PaginationRequest(int PageIndex = 0, int PageSize = 10);
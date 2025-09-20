using SharedKernel.Abstractions.Primitives;
using SharedKernel.Abstractions.Specifications;
using System.Linq.Expressions;

namespace SharedKernel.Specifications;

public abstract class Specification<TEntity> : ISpecification<TEntity> where TEntity : IEntity
{
    protected Specification(Expression<Func<TEntity, bool>>? criteria) =>
        Criteria = criteria;

    private Specification() { }

    // Filtering
    public Expression<Func<TEntity, bool>>? Criteria { get; }

    // Eager loading
    public List<Expression<Func<TEntity, object>>> Includes { get; } = [];

    // Sorting
    public Expression<Func<TEntity, object>>? OrderBy { get; private set; }
    public Expression<Func<TEntity, object>>? OrderByDescending { get; private set; }

    // Tracking
    public bool IsTracking { get; protected set; }

    // Split Query
    public bool IsSplitQuery { get; protected set; }

    // Pagination
    public bool IsPagingEnabled { get; protected set; }
    public int Skip { get; protected set; }
    public int Take { get; protected set; }

    protected void AddInclude(Expression<Func<TEntity, object>> includeExpression) =>
        Includes.Add(includeExpression);

    protected void AddOrderBy(Expression<Func<TEntity, object>> orderByExpression) =>
        OrderBy = orderByExpression;

    protected void AddOrderByDescending(Expression<Func<TEntity, object>> orderByDescendingExpression) =>
        OrderByDescending = orderByDescendingExpression;

    protected void ApplyPagination(int pageIndex, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageIndex, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);

        Skip = pageIndex * pageSize;
        Take = pageSize;
        IsPagingEnabled = true;
    }

    protected void EnableTracking() => IsTracking = true;

    protected void DisableTracking() => IsTracking = false;

    protected void EnableSplitQuery() => IsSplitQuery = true;
}
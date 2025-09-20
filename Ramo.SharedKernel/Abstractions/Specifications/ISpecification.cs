using SharedKernel.Abstractions.Primitives;
using System.Linq.Expressions;

namespace SharedKernel.Abstractions.Specifications;

public interface ISpecification<TEntity> where TEntity : IEntity
{
    // Filtering
    Expression<Func<TEntity, bool>>? Criteria { get; }

    // Eager loading
    List<Expression<Func<TEntity, object>>> Includes { get; }

    // Sorting
    Expression<Func<TEntity, object>>? OrderBy { get; }
    Expression<Func<TEntity, object>>? OrderByDescending { get; }

    // Tracking
    bool IsTracking { get; }

    // Split Query
    bool IsSplitQuery { get; }

    // Pagination
    bool IsPagingEnabled { get; }
    int Skip { get; }
    int Take { get; }
}
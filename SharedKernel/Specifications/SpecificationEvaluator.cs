using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Primitives;
using SharedKernel.Abstractions.Specifications;

namespace SharedKernel.Specifications;

public static class SpecificationEvaluator
{
    public static IQueryable<TEntity> GetQuery<TEntity>(IQueryable<TEntity> query, ISpecification<TEntity> specification)
        where TEntity : class, IEntity
    {
        // Filtering
        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        // Eager loading
        if (specification.Includes is not null && specification.Includes.Count != 0)
        {
            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));
        }

        // Sorting
        if (specification.OrderBy is not null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending is not null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        // Tracking
        if (!specification.IsTracking)
        {
            query = query.AsNoTracking();
        }

        // Split Query
        if (specification.IsSplitQuery)
        {
            query = query.AsSplitQuery();
        }

        // Pagination
        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip).Take(specification.Take);
        }

        return query;
    }
}
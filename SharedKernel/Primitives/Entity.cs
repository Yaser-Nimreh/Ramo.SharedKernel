using SharedKernel.Abstractions.Primitives;

namespace SharedKernel.Primitives;

public abstract class Entity<TId> : IEntity<TId>, IEquatable<Entity<TId>>
{
    protected Entity(TId id) => Id = id;

    protected Entity() { }

    public TId Id { get; private init; } = default!;

    public static bool operator ==(Entity<TId>? first, Entity<TId>? second) =>
        first is not null && second is not null && first.Equals(second);

    public static bool operator !=(Entity<TId>? first, Entity<TId>? second) =>
        !(first == second);

    public bool Equals(Entity<TId>? other)
    {
        if (other is null || other.GetType() != GetType())
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj is Entity<TId> entity && 
            EqualityComparer<TId>.Default.Equals(Id, entity.Id);
    }

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
}

public abstract class Entity : Entity<Guid>, IEntity
{
    protected Entity() : base() { }
    protected Entity(Guid id) : base(id) { }
}
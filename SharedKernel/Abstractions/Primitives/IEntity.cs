namespace SharedKernel.Abstractions.Primitives;

public interface IEntity<TId>
{
    TId Id { get; }
}

public interface IEntity : IEntity<Guid>;
using SharedKernel.Abstractions.Primitives;

namespace SharedKernel.Primitives;

public abstract class AggregateRoot<TId> : SoftDeletableEntity<TId>, IAggregateRoot
{
    protected AggregateRoot(TId id) : base(id) { }

    protected AggregateRoot() : base() { }

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();

    public void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}

public abstract class AggregateRoot : AggregateRoot<Guid>, IAggregateRoot
{
    protected AggregateRoot() : base() { }
    protected AggregateRoot(Guid id) : base(id) { }
}
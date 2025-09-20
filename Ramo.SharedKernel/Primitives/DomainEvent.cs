using SharedKernel.Abstractions.Primitives;

namespace SharedKernel.Primitives;

public abstract record DomainEvent : IDomainEvent
{
    protected DomainEvent(Guid id)
    {
        Id = id;
        OccurredOn = DateTime.UtcNow;
        EventType = GetType().AssemblyQualifiedName!;
    }

    protected DomainEvent() : this(Guid.NewGuid()) { }

    public Guid Id { get; init; }
    public DateTime OccurredOn { get; init; }
    public string EventType { get; init; }
}
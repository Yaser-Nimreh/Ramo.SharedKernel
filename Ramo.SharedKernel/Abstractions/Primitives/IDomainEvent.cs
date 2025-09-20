using MediatR;

namespace SharedKernel.Abstractions.Primitives;

public interface IDomainEvent : INotification
{
    Guid Id { get; init; }
    DateTime OccurredOn { get; init; }
    string EventType { get; init; }
}
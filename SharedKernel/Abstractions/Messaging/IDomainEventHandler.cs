using MediatR;
using SharedKernel.Abstractions.Primitives;

namespace SharedKernel.Abstractions.Messaging;

public interface IDomainEventHandler<in TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent;
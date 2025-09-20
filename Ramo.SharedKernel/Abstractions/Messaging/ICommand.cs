using MediatR;
using SharedKernel.Results;

namespace SharedKernel.Abstractions.Messaging;

public interface ICommand : IRequest<Result>;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
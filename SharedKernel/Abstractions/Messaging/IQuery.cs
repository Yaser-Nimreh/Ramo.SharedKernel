using MediatR;
using SharedKernel.Results;

namespace SharedKernel.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
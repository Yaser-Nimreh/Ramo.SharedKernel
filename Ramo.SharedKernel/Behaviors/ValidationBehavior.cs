using FluentValidation;
using FluentValidation.Results;
using MediatR;
using SharedKernel.Results;
using System.Reflection;

namespace SharedKernel.Behaviors;

internal sealed class ValidationBehavior<TCommand, TResponse>(
    IEnumerable<IValidator<TCommand>> validators)
    : IPipelineBehavior<TCommand, TResponse>
    where TCommand : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TCommand>> _validators = validators;

    public async Task<TResponse> Handle(
        TCommand command,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TCommand>(command);

        ValidationResult[] validationResults = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        ValidationFailure[] validationFailures = [.. validationResults
            .Where(validationResult => !validationResult.IsValid)
            .SelectMany(validationResult => validationResult.Errors)
            .Where(validationFailure => validationFailure is not null)];

        if (validationFailures.Length == 0)
        {
            return await next(cancellationToken);
        }

        Result[] results = [.. validationFailures.Select(validationfailure => Result.Failure(Error.Problem(validationfailure.PropertyName, validationfailure.ErrorMessage)))];

        ValidationError validationError = ValidationError.FromResults(results);

        return CreateFailedResult<TResponse>(validationError);
    }

    private static TResponse CreateFailedResult<TResult>(ValidationError validationError)
    {
        var responseType = typeof(TResponse);

        if (responseType == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(validationError);
        }

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var innerType = responseType.GetGenericArguments()[0];

            MethodInfo? failureMethod = typeof(Result<>)
                .MakeGenericType(innerType)
                .GetMethod(nameof(Result<object>.ValidationFailure));

            if (failureMethod is not null)
            {
                var result = failureMethod.Invoke(null, [validationError]);
                if (result is not null)
                {
                    return (TResponse)result;
                }
            }
        }

        throw new InvalidOperationException("Unable to create a validation result.");
    }
}
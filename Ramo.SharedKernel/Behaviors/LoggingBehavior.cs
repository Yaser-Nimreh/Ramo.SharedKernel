using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using SharedKernel.Primitives;
using SharedKernel.Results;
using System.Diagnostics;

namespace SharedKernel.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var responseName = typeof(TResponse).Name;

        string requestTypeLabel = GetRequestTypeLabel(typeof(TRequest));

        _logger.LogInformation("[START] Handling {RequestType} {RequestName} => {Response}, RequestData={RequestData}, at {DateTimeUtc}",
            requestTypeLabel, requestName, responseName, request, DateTime.UtcNow);

        var stopwatch = Stopwatch.StartNew();

        TResponse result = await next(cancellationToken);

        stopwatch.Stop();

        double seconds = stopwatch.Elapsed.TotalSeconds;

        if (seconds > 3)
        {
            _logger.LogWarning("[PERFORMANCE] {RequestType} {RequestName} took {TimeTaken} seconds.",
                requestTypeLabel, requestName, seconds);
        }

        if (result is Result r && r.IsFailure)
        {
            using (LogContext.PushProperty("Errors", result.Errors, true))
            {
                var parametersString = string.Join("; ",
                    typeof(TRequest).GetProperties()
                        //.Where(p => IsLoggableType(p.PropertyType))
                        .Select(p => $"{p.Name} = {p.GetValue(request)}"));

                _logger.LogError("[FAILURE] {RequestType} {RequestName} failed with errors {Errors}, Params: {Params}, at {DateTimeUtc}",
                    requestTypeLabel, requestName, result.Errors, parametersString.ToString(), DateTime.UtcNow);
            }
        }
        else
        {
            _logger.LogInformation("[SUCCESS] {RequestType} {RequestName} handled successfully at {DateTimeUtc}",
                requestTypeLabel, requestName, DateTime.UtcNow);
        }

        _logger.LogInformation("[END] Completed {RequestType} {RequestName} => {Response} in {ElapsedMilliseconds} ms at {DateTimeUtc}",
            requestTypeLabel, requestName, responseName, stopwatch.ElapsedMilliseconds, DateTime.UtcNow);

        return result;
    }

    private static string GetRequestTypeLabel(Type requestType)
    {
        var interfaces = requestType.GetInterfaces();

        if (interfaces.Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition().Name.StartsWith("ICommand", StringComparison.Ordinal)))
        {
            return "command";
        }

        if (interfaces.Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition().Name.StartsWith("IQuery", StringComparison.Ordinal)))
        {
            return "query";
        }

        return "request";
    }

    private static bool IsLoggableType(Type type)
    {
        return
            type.IsPrimitive
            || type == typeof(string)
            || type == typeof(Guid)
            || type.IsEnum
            || type == typeof(DateTime)
            || type == typeof(decimal)
            || IsValueObject(type);
    }

    private static bool IsValueObject(Type type)
    {
        while (type != null && type != typeof(object))
        {
            if (type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(ValueObject<>))
            {
                return true;
            }
            type = type.BaseType!;
        }
        return false;
    }
}
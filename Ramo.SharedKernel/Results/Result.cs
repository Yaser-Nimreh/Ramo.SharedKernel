using System.Diagnostics.CodeAnalysis;

namespace SharedKernel.Results;

public class Result
{
    protected internal Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
        Errors = [error];
    }

    protected internal Result(bool isSuccess, Error[] errors)
    {
        IsSuccess = isSuccess;
        Error = errors.Length > 0 ? errors[0] : Error.None;
        Errors = errors;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public Error[] Errors { get; }

    public static Result Success() => 
        new(true, Error.None);

    public static Result<TValue> Success<TValue>(TValue value) =>
        new(value, true, Error.None);

    public static Result Failure(Error error) => 
        new(false, error);

    public static Result Failure(Error[] errors) =>
        new(false, errors);

    public static Result<TValue> Failure<TValue>(Error error) =>
        new(default, false, error);

    public static Result<TValue> Failure<TValue>(Error[] errors) =>
        new(default, false, errors);

    public static Result Create(bool condition) =>
        condition ? Success() : Failure(Error.ConditionNotMet);

    public static Result<TValue> Create<TValue>(TValue? value) 
        => value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    public static Result<TValue> Create<TValue>(TValue? value, Error error)
        => value is not null ? Success(value) : Failure<TValue>(error);

    public static async Task<Result> FirstFailureOrSuccess(params Func<Task<Result>>[] results)
    {
        foreach (var resultTask in results)
        {
            var result = await resultTask();
            if (result.IsFailure)
            {
                return result;
            }
        }

        return Success();
    }

    private static Result<TValue> Ensure<TValue>(
        TValue value,
        Func<TValue, bool> predicate,
        Error error)
    {
        return predicate(value) ?
            Success(value) :
            Failure<TValue>(error);
    }

    public static Result<TValue> Ensure<TValue>(
        TValue value,
        params (Func<TValue, bool> predicate, Error error)[] functions)
    {
        var results = new List<Result<TValue>>();
        foreach ((Func<TValue, bool> predicate, Error error) in functions)
        {
            results.Add(Ensure(value, predicate, error));
        }

        return Combine(results.ToArray());
    }

    public static Result<TValue> Combine<TValue>(params Result<TValue>[] results)
    {
        if (results.Any(result => result.IsFailure))
        {
            return Failure<TValue>(
                [.. results.SelectMany(result => result.Errors).Distinct()]);
        }

        return Success(results[0].Value);
    }

    public static Result<(TFirst, TSecond)> Combine<TFirst, TSecond>(
        Result<TFirst> firstResult,
        Result<TSecond> secondResult)
    {
        if (firstResult.IsFailure)
        {
            return Failure<(TFirst, TSecond)>(firstResult.Errors);
        }

        if (secondResult.IsFailure)
        {
            return Failure<(TFirst, TSecond)>(secondResult.Errors);
        }

        return Success((firstResult.Value, secondResult.Value));
    }
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error) =>
        _value = value;

    protected internal Result(TValue? value, bool isSuccess, Error[] errors)
        : base(isSuccess, errors) =>
        _value = value;

    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can't be accessed.");

    public static implicit operator Result<TValue>(TValue? value) => Create(value);

    public static Result<TValue> ValidationFailure(Error error) =>
        new(default, false, error);
}

public static class ResultExtensions
{
    public static Result<TValue> Ensure<TValue>(
        this Result<TValue> result,
        Func<TValue, bool> predicate,
        Error error)
    {
        if (result.IsFailure)
        {
            return result;
        }

        return predicate(result.Value) ?
            result :
            Result.Failure<TValue>(error);
    }

    public static Result<TOut> Map<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> function)
    {
        return result.IsSuccess ?
            Result.Success(function(result.Value)) :
            Result.Failure<TOut>(result.Error);
    }

    public static Result<TOut> Bind<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> function)
    {
        return result.IsSuccess ?
            Result.Success(function(result.Value)) :
            Result.Failure<TOut>(result.Errors);
    }

    public static async Task<Result> Bind<TIn>(
        this Result<TIn> result,
        Func<TIn, Task<Result>> function)
    {
        if (result.IsFailure)
        {
            return Result.Failure(result.Errors);
        }

        return await function(result.Value);
    }

    public static async Task<Result<TOut>> Bind<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Task<Result<TOut>>> function)
    {
        if (result.IsFailure)
        {
            return Result.Failure<TOut>(result.Errors);
        }

        return await function(result.Value);
    }

    public static TOut Match<TOut>(
        this Result result,
        Func<TOut> onSuccess,
        Func<Result, TOut> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result);
    }

    public static TOut Match<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> onSuccess,
        Func<Result<TIn>, TOut> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result);
    }

    public static async Task<TOut> Match<TOut>(
        this Task<Result> resultTask,
        Func<TOut> onSuccess,
        Func<Result, TOut> onFailure)
    {
        var result = await resultTask;

        return result.IsSuccess ? onSuccess() : onFailure(result);
    }

    public static async Task<TOut> Match<TIn, TOut>(
        this Task<Result<TIn>> resultTask,
        Func<TIn, TOut> onSuccess,
        Func<Result<TIn>, TOut> onFailure)
    {
        var result = await resultTask;

        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result);
    }

    public static Result<TIn> Tap<TIn>(
        this Result<TIn> result,
        Action<TIn> action)
    {
        if (result.IsSuccess)
        {
            action(result.Value);
        }

        return result;
    }

    public static async Task<Result<TIn>> Tap<TIn>(
        this Result<TIn> result, 
        Func<Task> function)
    {
        if (result.IsSuccess)
        {
            await function();
        }

        return result;
    }

    public static async Task<Result<TIn>> Tap<TIn>(
        this Task<Result<TIn>> resultTask,
        Func<TIn, Task> function)
    {
        var result = await resultTask;

        if (result.IsSuccess)
        {
            await function(result.Value);
        }

        return result;
    }
}
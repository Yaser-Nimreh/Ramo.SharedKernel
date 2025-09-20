namespace SharedKernel.Results;

public class Error : IEquatable<Error>
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static readonly Error NullValue = new(
        "General.Null",
        "Null value was provided",
        ErrorType.Failure);
    public static readonly Error ConditionNotMet = new(
        "General.ConditionNotMet",
        "The specified condition was not met.",
        ErrorType.Failure);

    public Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    private Error() { }

    public string Code { get; } = string.Empty;

    public string Description { get; } = string.Empty;

    public ErrorType Type { get; }

    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error Problem(string code, string description) =>
        new(code, description, ErrorType.Problem);

    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    public static implicit operator string(Error error) => error.Code;

    public static bool operator ==(Error? first, Error? second) => 
        first is null && second is null || first is not null && second is not null && first.Equals(second);

    public static bool operator !=(Error? first, Error? second) => !(first == second);

    public bool Equals(Error? other) => other is not null && Code == other.Code && Description == other.Description && Type == other.Type;

    public override bool Equals(object? obj) => obj is Error error && Equals(error);

    public override int GetHashCode() => HashCode.Combine(Code, Description, Type);

    public override string ToString() => Code;

    public static implicit operator Result(Error error) => Result.Failure(error);
}
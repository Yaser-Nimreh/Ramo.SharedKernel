namespace SharedKernel.Results;

public sealed class ValidationError : Error
{
    private ValidationError(Error[] errors) : base(
        "Validation.General",
        "One or more validation errors occurred",
        ErrorType.Validation) => 
        Errors = errors;

    public Error[] Errors { get; }

    public static ValidationError FromResults(IEnumerable<Result> results) =>
        new([.. results.Where(r => r.IsFailure).SelectMany(r => r.Errors).Distinct()]);
}
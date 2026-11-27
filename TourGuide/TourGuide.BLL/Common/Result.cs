namespace TourGuide.BLL.Common;

// What every service method returns. A business failure is a failed Result, never an exception.
public class Result
{
    protected Result(bool succeeded, string? errorCode, string? message, List<ValidationError>? errors)
    {
        Succeeded = succeeded;
        ErrorCode = errorCode;
        Message = message;
        Errors = errors ?? new List<ValidationError>();
    }

    public bool Succeeded { get; }
    public string? ErrorCode { get; }
    public string? Message { get; }
    public List<ValidationError> Errors { get; }

    public static Result Ok() => new(true, null, null, null);

    public static Result Fail(string errorCode, string message) => new(false, errorCode, message, null);

    public static Result Invalid(List<ValidationError> errors) =>
        new(false, ErrorCodes.Validation, "Please fix the errors below.", errors);
}

public class Result<T> : Result
{
    private Result(bool succeeded, T? value, string? errorCode, string? message, List<ValidationError>? errors)
        : base(succeeded, errorCode, message, errors)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Ok(T value) => new(true, value, null, null, null);

    public static new Result<T> Fail(string errorCode, string message) => new(false, default, errorCode, message, null);

    public static new Result<T> Invalid(List<ValidationError> errors) =>
        new(false, default, ErrorCodes.Validation, "Please fix the errors below.", errors);
}

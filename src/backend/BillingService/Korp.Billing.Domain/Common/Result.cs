namespace Korp.Billing.Domain.Common;

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Unavailable
}

public class Result
{
    protected Result(bool success, string? error = null, ErrorType errorType = ErrorType.Validation)
    {
        IsSuccess = success;
        Error = error;
        ErrorType = errorType;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public string? Error { get; }

    public ErrorType ErrorType { get; }

    public static Result Success()
        => new(true);

    public static Result Failure(string error, ErrorType errorType = ErrorType.Validation)
        => new(false, error, errorType);
}

public class Result<T> : Result
{
    private Result(T? value, bool success, string? error = null, ErrorType errorType = ErrorType.Validation)
        : base(success, error, errorType)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(T value)
        => new(value, true);

    public static new Result<T> Failure(string error, ErrorType errorType = ErrorType.Validation)
        => new(default, false, error, errorType);
}
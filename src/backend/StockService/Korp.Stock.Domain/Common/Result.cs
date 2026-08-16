namespace Korp.Stock.Domain.Common;

public class Result
{
    protected Result(bool success, string? error = null)
    {
        IsSuccess = success;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public string? Error { get; }

    public static Result Success()
        => new(true);

    public static Result Failure(string error)
        => new(false, error);
}

public class Result<T> : Result
{
    private Result(T? value, bool success, string? error = null)
        : base(success, error)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(T value)
        => new(value, true);

    public static new Result<T> Failure(string error)
        => new(default, false, error);
}
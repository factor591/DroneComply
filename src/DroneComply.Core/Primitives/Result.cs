using System.Collections.Generic;

namespace DroneComply.Core.Primitives;

public sealed record Result<T>
{
    private Result(bool isSuccess, T? value, string? error, IReadOnlyDictionary<string, string[]>? validationErrors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        ValidationErrors = validationErrors;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public T? Value { get; }

    public string? Error { get; }

    public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; }

    public static Result<T> Success(T value) => new(true, value, null, null);

    public static Result<T> Failure(string error) => new(false, default, error, null);

    public static Result<T> ValidationFailure(string error, IReadOnlyDictionary<string, string[]> validationErrors) => new(false, default, error, validationErrors);
}

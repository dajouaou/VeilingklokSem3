namespace Veilingklok.Core.Shared;

public sealed class Result<T>
{
    public bool Success { get; private set; }
    public string? Error { get; private set; }
    public ErrorCode Code { get; private set; }
    public T? Value { get; private set; }

    public static Result<T> Ok(T value) => new()
    {
        Success = true,
        Value = value,
        Error = null,
        Code = ErrorCode.Unknown
    };

    public static Result<T> Fail(string error) => new()
    {
        Success = false,
        Error = error,
        Value = default,
        Code = ErrorCode.Unknown
    };

    public static Result<T> Fail(string error, ErrorCode code) => new()
    {
        Success = false,
        Error = error,
        Value = default,
        Code = code
    };
}
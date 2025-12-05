namespace Veilingklok.Core.Shared;

public sealed class Result<T>
{
    public bool Success { get; private set; }
    public string Error { get; private set; } = string.Empty;
    public T Value { get; private set; } = default!;

    public static Result<T> Ok(T value) => new() { Success = true, Value = value };
    public static Result<T> Fail(string error) => new() { Success = false, Error = error };
}
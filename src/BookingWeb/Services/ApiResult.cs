namespace BookingWeb.Services;

/// <summary>Result of an API call that can fail for business reasons (validation, slot taken, ...).</summary>
public record ApiResult<T>(T? Value, string? Error, int StatusCode)
{
    public bool IsSuccess => Error is null;

    public static ApiResult<T> Ok(T value, int statusCode = 200) => new(value, null, statusCode);

    public static ApiResult<T> Fail(string error, int statusCode) => new(default, error, statusCode);
}

namespace CustomerInsightHub.Application.Common;

public enum ResultStatus
{
    Success,
    Invalid,
    NotFound,
    Conflict
}

/// <summary>
/// The outcome of a use case. The service says WHAT happened; the controller decides
/// which HTTP status code that maps to. This keeps HTTP out of the Application layer.
/// </summary>
public class ServiceResult
{
    public ResultStatus Status { get; protected init; }
    public string? Message { get; protected init; }
    public IReadOnlyDictionary<string, string[]> Errors { get; protected init; } =
        new Dictionary<string, string[]>();

    public bool IsSuccess => Status == ResultStatus.Success;

    public static ServiceResult Ok() => new() { Status = ResultStatus.Success };
    public static ServiceResult NotFound(string? message = null) => new() { Status = ResultStatus.NotFound, Message = message };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Value { get; private init; }

    public static ServiceResult<T> Ok(T value) => new() { Status = ResultStatus.Success, Value = value };

    public static ServiceResult<T> Invalid(IReadOnlyDictionary<string, string[]> errors) =>
        new() { Status = ResultStatus.Invalid, Errors = errors };

    public static new ServiceResult<T> NotFound(string? message = null) =>
        new() { Status = ResultStatus.NotFound, Message = message };

    public static ServiceResult<T> Conflict(string message) =>
        new() { Status = ResultStatus.Conflict, Message = message };
}


public class ServiceResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public ErrorTypeEnum? ErrorType { get; set; }

    public static ServiceResult Ok()
        => new() { Success = true };

    public static ServiceResult NotFound(string error)
        => new() { Success = false, Error = error, ErrorType = ErrorTypeEnum.NotFound };

    public static ServiceResult ValidationError(string error)
        => new() { Success = false, Error = error, ErrorType = ErrorTypeEnum.Validation };

    public static ServiceResult Conflict(string error)
        => new() { Success = false, Error = error, ErrorType = ErrorTypeEnum.Conflict };
}
public class ServiceResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public ErrorTypeEnum ErrorType { get; set; }

    public static ServiceResult<T> Ok(T data)
        => new() { Success = true, Data = data, ErrorType = ErrorTypeEnum.None };

    public static ServiceResult<T> NotFound(string error)
        => new() { Success = false, Error = error, ErrorType = ErrorTypeEnum.NotFound };

    public static ServiceResult<T> ValidationError(string error)
        => new() { Success = false, Error = error, ErrorType = ErrorTypeEnum.Validation };

    public static ServiceResult<T> Conflict(string error)
        => new() { Success = false, Error = error, ErrorType = ErrorTypeEnum.Conflict };
}
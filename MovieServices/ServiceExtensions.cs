using Microsoft.Extensions.Logging;

public static class ServiceResultFactory
{
    public static ServiceResult Ok()
        => new()
        {
            Success = true
        };

    public static ServiceResult<T> Ok<T>(T data)
        => new()
        {
            Success = true,
            Data = data
        };

    public static ServiceResult Fail(
        ILogger logger,
        ErrorTypeEnum type,
        string message,
        string operation)
    {
        Log(logger, type, message, operation);

        return new ServiceResult
        {
            Success = false,
            Error = message,
            ErrorType = type
        };
    }

    public static ServiceResult<T> Fail<T>(
        ILogger logger,
        ErrorTypeEnum type,
        string message,
        string operation)
    {
        Log(logger, type, message, operation);

        return new ServiceResult<T>
        {
            Success = false,
            Error = message,
            ErrorType = type
        };
    }

    private static void Log(
        ILogger logger,
        ErrorTypeEnum type,
        string message,
        string operation)
    {
        switch (type)
        {
            case ErrorTypeEnum.NotFound:
                logger.LogWarning("{Operation} failed: {Message}", operation, message);
                break;

            case ErrorTypeEnum.Validation:
                logger.LogWarning("{Operation} validation error: {Message}", operation, message);
                break;

            case ErrorTypeEnum.Conflict:
                logger.LogWarning("{Operation} conflict: {Message}", operation, message);
                break;

            default:
                logger.LogError("{Operation} unexpected error: {Message}", operation, message);
                break;
        }
    }
}
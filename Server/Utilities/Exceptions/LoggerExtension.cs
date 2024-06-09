namespace Server.Utilities.Exceptions;

public static class LoggerExtension
{
    private static string GetTimestamp() => DateTime.UtcNow.ToString("HH:mm:ss");

    public static void LogWithTimestamp(this ILogger logger, LogLevel logLevel, string message)
    {
        logger.Log(logLevel, $"[{GetTimestamp()}] {message}");
    }

    public static void LogInformationWithTimestamp(this ILogger logger, string message)
    {
        logger.LogInformation($"[{GetTimestamp()}] {message}");
    }

    public static void LogWarningWithTimestamp(this ILogger logger, string message)
    {
        logger.LogWarning($"[{GetTimestamp()}] {message}");
    }

    public static void LogErrorWithTimestamp(this ILogger logger, string message)
    {
        logger.LogError($"[{GetTimestamp()}] {message}");
    }
}

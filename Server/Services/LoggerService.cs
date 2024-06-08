using Server.Services.Interfaces;

namespace Server.Services;

public class LoggerService(ILogger logger, ConsoleService consoleService) : ILoggerService
{
    private readonly ILogger _logger = logger;
    private readonly ConsoleService _consoleService = consoleService;

    public void LogAndSendMessage(string clientId, string message, LogLevel logLevel)
    {
        var timestampedMessage = $"[{DateTime.UtcNow:HH:mm:ss}] {message}";
        _consoleService.SendMessage(clientId, timestampedMessage);

        switch (logLevel)
        {
            case LogLevel.Information:
                _logger.LogInformation(timestampedMessage);
                break;
            case LogLevel.Warning:
                _logger.LogWarning(timestampedMessage);
                break;
            case LogLevel.Error:
                _logger.LogError(timestampedMessage);
                break;
        }
    }
}

namespace Server.Services.Interfaces;

public interface ILoggerService
{
    public Task LogAndSendMessage<T>(ILogger<T> logger, string clientId, string message, LogLevel logLevel);
}

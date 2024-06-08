namespace Server.Services.Interfaces;

public interface ILoggerService
{
    public void LogAndSendMessage(string clientId, string message, LogLevel logLevel);
}

using Server.Models;

namespace Server.Utilities.Logs;

public class LoggerService(
    IMessageSender messageSender,
    LoggerHelper loggerHelper
    ) : IMessageSender
{
    private readonly IMessageSender _messageSender = messageSender;
    private readonly LoggerHelper _loggerHelper = loggerHelper;

    public async Task SendLogAsync<T>(ILogger<T> logger, string connectionGuid, string message, LogLevel logLevel)
    {
        var timestampedMessage = _loggerHelper.FormatMessageWithTimestamp(message);
        var messageModel = _loggerHelper.CreateMessageModel(connectionGuid, timestampedMessage);

        await SendMessageAsync(messageModel);
        LogMessage(logger, message, logLevel);
    }

    public async Task SendMessageAsync(Message messageModel)
    {
        await _messageSender.SendMessageAsync(messageModel);
    }

    private void LogMessage<T>(ILogger<T> logger, string message, LogLevel logLevel)
    {
        switch (logLevel)
        {
            case LogLevel.Information:
                logger.LogInformation(message);
                break;
            case LogLevel.Warning:
                logger.LogWarning(message);
                break;
            case LogLevel.Error:
                logger.LogError(message);
                break;
        }
    }
}

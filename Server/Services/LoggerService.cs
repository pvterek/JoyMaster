using Microsoft.AspNetCore.SignalR;
using Server.Models;
using Server.Services.Interfaces;
using Server.Utilities.Hubs;

namespace Server.Services;

public class LoggerService(IHubContext<ConsoleHub> hubContext) : ILoggerService
{
    private readonly IHubContext<ConsoleHub> _hubContext = hubContext;

    public async Task LogAndSendMessage<T>(ILogger<T> logger, string clientId, string message, LogLevel logLevel)
    {
        var timestampedMessage = FormatMessageWithTimestamp(message);
        var messageModel = CreateMessageModel(clientId, timestampedMessage);

        await SendMessage(messageModel);
        LogMessage(logger, timestampedMessage, logLevel);
    }

    private async Task SendMessage(MessageModel messageModel)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", messageModel);
    }

    private string FormatMessageWithTimestamp(string message)
    {
        return $"[{DateTime.UtcNow:HH:mm:ss}] {message}";
    }

    private MessageModel CreateMessageModel(string clientId, string message)
    {
        return new MessageModel { ClientId = clientId, Message = message };
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

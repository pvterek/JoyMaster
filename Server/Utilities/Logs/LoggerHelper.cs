using Server.Models;

namespace Server.Utilities.Logs;

public class LoggerHelper
{
    public string FormatMessageWithTimestamp(string message)
    {
        return $"[{DateTime.UtcNow:HH:mm:ss}] {message}";
    }

    public MessageModel CreateMessageModel(string clientId, string message)
    {
        return new MessageModel { ClientId = clientId, Message = message };
    }
}

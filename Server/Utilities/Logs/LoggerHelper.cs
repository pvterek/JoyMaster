using Server.Models;

namespace Server.Utilities.Logs;

public class LoggerHelper
{
    public string FormatMessageWithTimestamp(string message)
    {
        return $"[{DateTime.UtcNow:HH:mm:ss}] {message}";
    }

    public Message CreateMessageEntity(string connectionGuid, string message)
    {
        return new Message
        {
            ConnectionGuid = connectionGuid,
            MessageContent = message
        };
    }
}

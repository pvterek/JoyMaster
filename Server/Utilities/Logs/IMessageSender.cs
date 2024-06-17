using Server.Models;

namespace Server.Utilities.Logs;

public interface IMessageSender
{
    Task SendMessageAsync(MessageModel messageModel);
}

using Microsoft.AspNetCore.SignalR;
using Server.Models;
using Server.Utilities.Hubs;

namespace Server.Utilities.Logs;

public class MessageSender(IHubContext<ConsoleHub> hubContext) : IMessageSender
{
    private readonly IHubContext<ConsoleHub> _hubContext = hubContext;

    public async Task SendMessageAsync(Message messageModel)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", messageModel);
    }
}

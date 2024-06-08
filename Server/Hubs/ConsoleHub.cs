using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs;

public class ConsoleHub() : Hub
{
    public async Task SendMessageToAll(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}

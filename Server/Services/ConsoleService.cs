using Microsoft.AspNetCore.SignalR;
using Server.Hubs;

namespace Server.Services;

public class ConsoleService(IHubContext<ConsoleHub> hubContext) //merge into one class with LoggerService
{
    private readonly IHubContext<ConsoleHub> _hubContext = hubContext;

    public async void SendMessage(string clientId, string message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
    }
}

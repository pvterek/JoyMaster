using Microsoft.AspNetCore.SignalR;

namespace Server.Utilities.Hubs;

public class ScreenHub : Hub
{
    public async Task SendScreenData(byte[] imageData)
    {
        await Clients.All.SendAsync("ReceiveScreenData", imageData);
    }
}

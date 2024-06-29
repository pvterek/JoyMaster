using Microsoft.AspNetCore.SignalR;
using Server.Entities;
using Server.Models;
using Server.Utilities.Hubs;

namespace Server.Utilities.Logs;

public class SenderService(
    IHubContext<CommandHub> commandHubContext,
    IHubContext<ImageHub> imageHubContext
    ) : IMessageSender, IImageSender
{
    private readonly IHubContext<CommandHub> _commandHubContext = commandHubContext;
    private readonly IHubContext<ImageHub> _imageHubContext = imageHubContext;

    public async Task SendMessageAsync(Message message)
    {
        await _commandHubContext.Clients.All.SendAsync("ReceiveMessage", message);
    }

    public async Task SendImageAsync(ImageData imageData)
    {
        await _imageHubContext.Clients.All.SendAsync("ReceiveImageData", imageData);
    }
}

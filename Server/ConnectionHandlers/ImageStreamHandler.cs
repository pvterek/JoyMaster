using Grpc.Core;
using Microsoft.AspNetCore.SignalR;
using Server.Protos;
using Server.Utilities.Hubs;

namespace Server.ConnectionHandlers;

public class ImageStreamHandler(
    IHubContext<ScreenHub> hubContext
    ) : ImageStreamer.ImageStreamerBase
{
    private readonly IHubContext<ScreenHub> _hubContext = hubContext;

    public override async Task ImageStream(
        IAsyncStreamReader<DesktopFrame> requestStream,
        IServerStreamWriter<Empty> responseStream,
        ServerCallContext context)
    {
        await foreach (var frame in requestStream.ReadAllAsync())
        {
            var base64Image = Convert.ToBase64String(frame.Image.ToByteArray());
            await _hubContext.Clients.All.SendAsync("ReceiveScreenData", base64Image);
        }
    }

}

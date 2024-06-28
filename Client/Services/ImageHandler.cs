namespace Client.Services;

public class ImageHandler(
    ConnectionService clientService,
    ScreenCaptureService screenCaptureService)
{
    private readonly ConnectionService _clientService = clientService;
    private readonly ScreenCaptureService _screenCaptureService = screenCaptureService;

    public async Task StreamImagesAsync(CancellationToken cancellationToken)
    {
        var client = _clientService.ConfigureImageStreamerClient();

        using var call = client.ImageStream(cancellationToken: cancellationToken);
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var frame = _screenCaptureService.CaptureDesktopFrame();
                await call.RequestStream.WriteAsync(frame, cancellationToken);
                await Task.Delay(100, cancellationToken);
            }
        }
        finally
        {
            await call.RequestStream.CompleteAsync();
        }
    }
}

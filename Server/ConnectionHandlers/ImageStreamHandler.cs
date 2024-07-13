using Grpc.Core;
using Server.Protos;
using Server.Utilities;
using Server.Utilities.Logs;

namespace Server.ConnectionHandlers;

public class ImageStreamHandler(
    ILogger<ImageStreamHandler> logger,
    LoggerService loggerService,
    IImageSender imageSender,
    ImageDataHelper imageDataHelper
    ) : ImageStreamer.ImageStreamerBase
{
    private readonly ILogger<ImageStreamHandler> _logger = logger;
    private readonly LoggerService _loggerService = loggerService;
    private readonly IImageSender _imageSender = imageSender;
    private readonly ImageDataHelper _imageDataHelper = imageDataHelper;

    private string _connectionGuid = null!;

    public override async Task ImageStream(
        IAsyncStreamReader<DesktopFrame> requestStream,
        IServerStreamWriter<Empty> responseStream,
        ServerCallContext context)
    {
        try
        {
            await HandleConnectionAsync(requestStream);
        }
        catch (Exception ex)
        {
            await _loggerService.SendLogAsync(
            _logger,
            _connectionGuid,
            ex.ToString(),
            LogLevel.Error);
        }
    }

    private async Task HandleConnectionAsync(IAsyncStreamReader<DesktopFrame> requestStream)
    {
        await foreach (var request in requestStream.ReadAllAsync())
        {
            _connectionGuid = request.Id;

            var imageData = _imageDataHelper.CreateImageDataEntity(request.Id, request.Image);
            await _imageSender.SendImageAsync(imageData);
        }
    }
}

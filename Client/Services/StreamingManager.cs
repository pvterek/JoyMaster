using Client.ConnectionHandlers;

namespace Client.Services;

public class StreamingManager(ImageStreamHandler imageStreamHandler)
{
    private readonly ImageStreamHandler _imageStreamHandler = imageStreamHandler;
    private CancellationTokenSource _streamingCts = null!;
    private Task _streamingTask = null!;

    public void CreateStreamingTask()
    {
        if (_streamingTask == null || _streamingTask.IsCompleted)
        {
            _streamingCts?.Cancel();
            _streamingCts = new CancellationTokenSource();
            _streamingTask = _imageStreamHandler.StreamImagesAsync(_streamingCts.Token);
        }
    }

    public void CancelStreaming()
    {
        _streamingCts?.Cancel();
    }
}

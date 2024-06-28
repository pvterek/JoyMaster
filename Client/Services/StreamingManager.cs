using Client.ConnectionHandlers;

namespace Client.Services;

public class StreamingManager(ImageStreamHandler imageHandler)
{
    private readonly ImageStreamHandler _imageHandler = imageHandler;
    private CancellationTokenSource _streamingCts = null!;
    private Task _streamingTask = null!;

    public void ManageStreamingTask()
    {
        if (_streamingTask == null || _streamingTask.IsCompleted)
        {
            _streamingCts?.Cancel();
            _streamingCts = new CancellationTokenSource();
            _streamingTask = _imageHandler.StreamImagesAsync(_streamingCts.Token);
        }
    }

    public void CancelStreaming()
    {
        _streamingCts?.Cancel();
    }
}

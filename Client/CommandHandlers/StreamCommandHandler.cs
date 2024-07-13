using Client.Services;
using Grpc.Core;
using Server.Protos;

namespace Client.CommandHandlers;

internal class StreamCommandHandler(
    StreamingManager streamingManager
    ) : ICommandHandler
{
    private readonly StreamingManager _streamingManager = streamingManager;

    public async Task ExecuteAsync(
        Response? response = null,
        IClientStreamWriter<Request>? requestStream = null)
    {
        ArgumentNullException.ThrowIfNull(response);
        var parameters = response.Parameters.ToLower().Trim();

        if (parameters.Equals(AppConstants.EnableParam))
        {
            _streamingManager.CreateStreamingTask();
        }
        else if (parameters.Equals(AppConstants.DisableParam))
        {
            _streamingManager.CancelStreaming();
        }

        await Task.CompletedTask;
    }
}

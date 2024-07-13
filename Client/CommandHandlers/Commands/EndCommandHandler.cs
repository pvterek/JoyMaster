using Client.CommandHandlers.Interfaces;
using Client.Services;
using Grpc.Core;
using Server.Protos;
using System.Windows;

namespace Client.CommandHandlers.Commands;

internal class EndCommandHandler(
    StreamingManager streamingManager
    ) : ICommandHandler
{
    private readonly StreamingManager _streamingManager = streamingManager;

    public async Task ExecuteAsync(
        Response? response = null,
        IClientStreamWriter<Request>? requestStream = null)
    {
        ArgumentNullException.ThrowIfNull(requestStream);
        _streamingManager.CancelStreaming();
        await requestStream.CompleteAsync();
        Application.Current.Shutdown();
    }
}

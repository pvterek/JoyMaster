using Client.Services;
using Grpc.Core;
using Server.Protos;
using System.Windows;

namespace Client.ConnectionHandlers;

public class CommandHandler(
    StreamingManager streamingManager,
    RequestHandler requestHandler)
{
    private readonly StreamingManager _streamingManager = streamingManager;
    private readonly RequestHandler _requestHandler = requestHandler;

    public async Task HandleCommandAsync(Response response, IClientStreamWriter<Request> requestStream)
    {
        switch (response.Command)
        {
            case AppConstants.EndCommand:
                _streamingManager.CancelStreaming();
                await requestStream.CompleteAsync();
                Application.Current.Shutdown();
                break;

            case AppConstants.StreamCommand:
                _streamingManager.CreateStreamingTask();
                break;

            case AppConstants.AlertCommand:
                MessageBox.Show(response.Parameters, "JoyMaster", MessageBoxButton.OK);
                break;

            default:
                await _requestHandler.ExecuteCommandAsync(response.Parameters, requestStream);
                break;
        }
    }
}

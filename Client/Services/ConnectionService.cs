using Grpc.Core;
using Server.Protos;
using System.Windows;

namespace Client.Services;

internal class ConnectionService
{
    private CancellationTokenSource _streamingCts = null!;
    private Task _streamingTask = null!;
    public async Task HandleConnectionAsync()
    {
        var client = ConfigureClient();
        using var call = client.CommandStream();

        var initialRequest = new Request
        {
            Id = ConnectionGuid,
            Name = ClientName,
            IsInitial = true
        };

        await call.RequestStream.WriteAsync(initialRequest);

        try
        {
            await foreach (var response in call.ResponseStream.ReadAllAsync())
            {
                await HandleCommandAsync(response, call.RequestStream);
            }
        }
        finally
        {
            await call.RequestStream.CompleteAsync();
        }
    }

    private async Task HandleCommandAsync(Response response, IClientStreamWriter<Request> requestStream)
    {
        var command = response.Command;
        var parameters = response.Parameters;

        switch (command)
        {
            case AppConstants.EndCommand:
                _streamingCts?.Cancel();
                await requestStream.CompleteAsync();
                break;

            case AppConstants.StreamCommand:
                if (_streamingTask == null || _streamingTask.IsCompleted)
                {
                    _streamingCts?.Cancel();
                    _streamingCts = new CancellationTokenSource();
                    _streamingTask = DesktopStreaming.Run(_streamingCts.Token);
                }
                break;

            case AppConstants.AlertCommand:
                MessageBox.Show(parameters, "JoyMaster", MessageBoxButton.OK);
                break;

            default:
                //if (_streamingTask != null && !_streamingTask.IsCompleted)
                var executionResult = await CommandExecutor.ExecuteCommand(parameters);
                var request = new Request
                {
                    Id = ConnectionGuid,
                    Message = executionResult,
                    IsInitial = false
                };
                await requestStream.WriteAsync(request);
                break;
        }
    }
}

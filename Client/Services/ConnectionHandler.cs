using Grpc.Core;
using Server.Protos;
using System.Windows;

namespace Client.Services;

public class ConnectionHandler(
    ConnectionService clientService,
    CommandExecutor commandExecutor,
    ImageHandler desktopStreaming)
{
    private readonly ConnectionService _clientService = clientService;
    private readonly CommandExecutor _commandExecutor = commandExecutor;
    private readonly ImageHandler _desktopStreaming = desktopStreaming;
    private CancellationTokenSource _streamingCts = null!;
    private Task _streamingTask = null!;

    public async Task HandleConnectionAsync()
    {
        var client = _clientService.ConfigureCommandStreamerClient();
        using var call = client.CommandStream();
        await SendInitialRequestAsync(call.RequestStream);

        try
        {
            await ProcessResponseStreamAsync(call.ResponseStream.ReadAllAsync(), call.RequestStream);
        }
        finally
        {
            await call.RequestStream.CompleteAsync();
        }
    }

    private async Task SendInitialRequestAsync(IClientStreamWriter<Request> requestStream)
    {
        var initialRequest = new Request
        {
            Id = _clientService.ConnectionGuid,
            Name = _clientService.ClientName,
            IsInitial = true
        };
        await requestStream.WriteAsync(initialRequest);
    }

    private async Task ProcessResponseStreamAsync(IAsyncEnumerable<Response> responseStream, IClientStreamWriter<Request> requestStream)
    {
        await foreach (var response in responseStream)
        {
            await HandleCommandAsync(response, requestStream);
        }
    }

    private async Task HandleCommandAsync(Response response, IClientStreamWriter<Request> requestStream)
    {
        switch (response.Command)
        {
            case AppConstants.EndCommand:
                _streamingCts?.Cancel();
                await requestStream.CompleteAsync();
                break;

            case AppConstants.StreamCommand:
                ManageStreamingTask();
                break;

            case AppConstants.AlertCommand:
                MessageBox.Show(response.Parameters, "JoyMaster", MessageBoxButton.OK);
                break;

            default:
                await ExecuteAndSendCommandAsync(response.Parameters, requestStream);
                break;
        }
    }

    private void ManageStreamingTask()
    {
        if (_streamingTask == null || _streamingTask.IsCompleted)
        {
            _streamingCts?.Cancel();
            _streamingCts = new CancellationTokenSource();
            _streamingTask = _desktopStreaming.StreamImagesAsync(_streamingCts.Token);
        }
    }

    private async Task ExecuteAndSendCommandAsync(string parameters, IClientStreamWriter<Request> requestStream)
    {
        var executionResult = await _commandExecutor.ExecuteCommand(parameters);
        var request = new Request
        {
            Id = _clientService.ConnectionGuid,
            Message = executionResult,
            IsInitial = false
        };
        await requestStream.WriteAsync(request);
    }
}

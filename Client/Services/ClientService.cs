using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Server.Protos;
using System.Windows;
using static Server.Protos.CommandStreamer;

namespace Client.Services;

internal class ClientService
{
    private readonly string ClientName = Environment.UserName;
    public static readonly string ConnectionGuid = Guid.NewGuid().ToString();

    private CancellationTokenSource _streamingCts = null!;
    private Task _streamingTask = null!;

    public async Task Run()
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
                var input = response.Message;
                await HandleCommandAsync(input, call.RequestStream);
            }
        }
        finally
        {
            await call.RequestStream.CompleteAsync();
        }
    }

    private CommandStreamerClient ConfigureClient()
    {
        var defaultMethodConfig = new MethodConfig
        {
            Names = { MethodName.Default },
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 20,
                InitialBackoff = TimeSpan.FromSeconds(1),
                MaxBackoff = TimeSpan.FromMinutes(20),
                BackoffMultiplier = 5,
                RetryableStatusCodes =
                {
                    StatusCode.Unavailable,
                    //StatusCode.DeadlineExceeded
                }
            }
        };

        var serviceConfig = new ServiceConfig
        {
            MethodConfigs = { defaultMethodConfig }
        };

        var channel = GrpcChannel.ForAddress("https://localhost:7018", new GrpcChannelOptions
        {
            ServiceConfig = serviceConfig
        });

        return new CommandStreamerClient(channel);
    }

    private async Task HandleCommandAsync(string command, IClientStreamWriter<Request> requestStream)
    {
        switch (command)
        {
            case AppConstants.EndCommand:
                _streamingCts?.Cancel();
                await requestStream.CompleteAsync();
                break;

            case AppConstants.EnableStreamCommand:
                if (_streamingTask == null || _streamingTask.IsCompleted)
                {
                    _streamingCts?.Cancel();
                    _streamingCts = new CancellationTokenSource();
                    _streamingTask = DesktopStreaming.Run(_streamingCts.Token);
                }
                break;

            case AppConstants.AlertCommand:
                MessageBox.Show("test", string.Empty, MessageBoxButton.OK);
                break;

            default:
                //if (_streamingTask != null && !_streamingTask.IsCompleted)
                var executionResult = await CommandService.ExecuteCommand(command);
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

using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using Server.Protos;
using static Server.Protos.Handler;

namespace Client.Services;

internal class ClientService
{
    private readonly string ClientName = Environment.UserName;
    private readonly string ConnectionGuid = Guid.NewGuid().ToString();

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

    private HandlerClient ConfigureClient()
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

        return new HandlerClient(channel);
    }

    private async Task HandleCommandAsync(string command, IClientStreamWriter<Request> requestStream)
    {
        switch (command)
        {
            case AppConstants.EndCommand:
                await requestStream.CompleteAsync();
                break;
            default:
                var executionResult = await CommandService.ExecuteCommand(command);
                var request = new Request
                {
                    Id = ConnectionGuid,
                    //Name = ClientName,
                    Message = executionResult,
                    IsInitial = false
                };
                await requestStream.WriteAsync(request);
                break;
        }
    }
}

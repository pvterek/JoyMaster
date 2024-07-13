using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using static Client.Protos.CommandStreamer;
using static Client.Protos.ImageStreamer;

namespace Client.Services;

public class ConnectionService
{
    public readonly string ClientName = Environment.UserName;
    public readonly string ConnectionGuid;

    public ConnectionService()
    {
        ConnectionGuid = Guid.NewGuid().ToString();
    }

    public CommandStreamerClient ConfigureCommandStreamerClient()
    {
        return ConfigureClient<CommandStreamerClient>();
    }

    public ImageStreamerClient ConfigureImageStreamerClient()
    {
        return ConfigureClient<ImageStreamerClient>();
    }

    private T ConfigureClient<T>() where T : class
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

        if (Activator.CreateInstance(typeof(T), channel) is not T client)
        {
            throw new InvalidOperationException($"Unable to create an instance of type {typeof(T).FullName}.");
        }

        return client;
    }
}

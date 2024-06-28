using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using static Server.Protos.ImageStreamer;

namespace Client.Services;

public class DesktopStreaming
{
    public static async Task Run(CancellationToken cancellationToken)
    {
        var client = ConfigureClient();
        using var call = client.ImageStream(cancellationToken: cancellationToken);
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var frame = ScreenCaptureService.CaptureDesktopFrame();
                await call.RequestStream.WriteAsync(frame, cancellationToken);
                await Task.Delay(100, cancellationToken);
            }
        }
        finally
        {
            await call.RequestStream.CompleteAsync();
        }
    }

    private static ImageStreamerClient ConfigureClient()
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

        return new ImageStreamerClient(channel);
    }
}

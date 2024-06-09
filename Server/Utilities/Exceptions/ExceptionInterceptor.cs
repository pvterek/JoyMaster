using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Server.Utilities.Exceptions;

public class ExceptionInterceptor(ILogger<ExceptionInterceptor> logger) : Interceptor
{
    private readonly ILogger<ExceptionInterceptor> _logger = logger;

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            await continuation(requestStream, responseStream, context);
        }
        catch (Exception e)
        {
            throw e.Handle(context, _logger);
        }
    }
}

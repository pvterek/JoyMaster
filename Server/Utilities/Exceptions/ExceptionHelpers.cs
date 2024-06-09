using Grpc.Core;

namespace Server.Utilities.Exceptions;

public static class ExceptionHelpers
{
    public static RpcException Handle<T>(this Exception exception, ServerCallContext context, ILogger<T> logger) =>
        exception switch
        {
            TimeoutException => HandleTimeoutException((TimeoutException)exception, logger),
            RpcException => HandleRpcException((RpcException)exception, logger),
            _ => HandleDefault(exception, logger)
        };

    private static RpcException HandleTimeoutException<T>(TimeoutException exception, ILogger<T> logger)
    {
        logger.LogError(exception, "A timeout occurred");
        Status status;

        status = new Status(StatusCode.Internal, "An external resource did not answer within the time limit");

        return new RpcException(status);
    }

    private static RpcException HandleRpcException<T>(RpcException exception, ILogger<T> logger)
    {
        logger.LogError(exception, "An error occurred");
        var trailers = exception.Trailers;
        return new RpcException(new Status(exception.StatusCode, exception.Message), trailers);
    }

    private static RpcException HandleDefault<T>(Exception exception, ILogger<T> logger)
    {
        logger.LogError(exception, "An error occurred");
        return new RpcException(new Status(StatusCode.Internal, exception.Message));
    }
}

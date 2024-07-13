using Client.CommandHandlers.Interfaces;
using Client.ConnectionHandlers;
using Grpc.Core;
using Server.Protos;

namespace Client.CommandHandlers.Commands;

internal class SendCommandHandler(
    RequestHandler requestHandler
    ) : ICommandHandler
{
    private readonly RequestHandler _requestHandler = requestHandler;

    public async Task ExecuteAsync(
        Response? response = null,
        IClientStreamWriter<Request>? requestStream = null)
    {
        ArgumentNullException.ThrowIfNull(response);
        ArgumentNullException.ThrowIfNull(requestStream);

        await _requestHandler.ExecuteCommandAsync(response.Parameters, requestStream);
    }
}

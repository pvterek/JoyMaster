using Client.Services;
using Grpc.Core;
using Server.Protos;

namespace Client.ConnectionHandlers;

public class CommandStreamHandler(
    ConnectionService clientService,
    CommandHandler commandHandler,
    RequestHandler requestHandler)
{
    private readonly ConnectionService _clientService = clientService;
    private readonly CommandHandler _commandHandler = commandHandler;
    private readonly RequestHandler _requestHandler = requestHandler;

    public async Task HandleConnectionAsync()
    {
        var client = _clientService.ConfigureCommandStreamerClient();
        using var call = client.CommandStream();
        await _requestHandler.SendInitialRequestAsync(call.RequestStream);

        try
        {
            await ProcessResponseStreamAsync(call.ResponseStream.ReadAllAsync(), call.RequestStream);
        }
        finally
        {
            await call.RequestStream.CompleteAsync();
        }
    }

    private async Task ProcessResponseStreamAsync(IAsyncEnumerable<Response> responseStream, IClientStreamWriter<Request> requestStream)
    {
        await foreach (var response in responseStream)
        {
            await _commandHandler.HandleCommandAsync(response, requestStream);
        }
    }
}
using Client.Services;
using Grpc.Core;
using Server.Protos;

namespace Client.ConnectionHandlers;

public class RequestHandler(
    ConnectionService clientService,
    CommandExecutor commandExecutor)
{
    private readonly ConnectionService _clientService = clientService;
    private readonly CommandExecutor _commandExecutor = commandExecutor;

    public async Task SendInitialRequestAsync(IClientStreamWriter<Request> requestStream)
    {
        var initialRequest = new Request
        {
            Id = _clientService.ConnectionGuid,
            Name = _clientService.ClientName,
            IsInitial = true
        };
        await requestStream.WriteAsync(initialRequest);
    }

    public async Task ExecuteCommandAsync(string parameters, IClientStreamWriter<Request> requestStream)
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

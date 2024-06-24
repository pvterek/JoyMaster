using Grpc.Core;
using Server.Protos;
using Server.Services.Interfaces;
using Server.Utilities.Logs;

namespace Server.Services;

internal class HandlerService(
    ILogger<HandlerService> logger,
    IConnectionService connectionService,
    LoggerService loggerService,
    ClientService clientService
    ) : Handler.HandlerBase
{
    private readonly ILogger<HandlerService> _logger = logger;
    private readonly IConnectionService _connectionService = connectionService;
    private readonly LoggerService _loggerService = loggerService;
    private readonly ClientService _clientService = clientService;
    private string _clientAddress = null!;
    private string _connectionGuid = null!;

    public override async Task CommandStream(
        IAsyncStreamReader<Request> requestStream,
        IServerStreamWriter<Response> responseStream,
        ServerCallContext context)
    {
        _clientAddress = context.Host;

        try
        {
            await HandleConnectionAsync(requestStream, responseStream);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(ex, context);
        }
    }

    private async Task HandleConnectionAsync(
        IAsyncStreamReader<Request> requestStream,
        IServerStreamWriter<Response> responseStream)
    {
        await foreach (var request in requestStream.ReadAllAsync())
        {
            _connectionGuid = request.Id;

            if (!request.IsInitial)
            {
                await _loggerService.SendLogAsync(_logger,
                    request.Id,
                    request.Message,
                    LogLevel.Information);
                continue;
            }

            var client = await _clientService.GetClientAsync(request.Name, _clientAddress)
                ?? await _clientService.RegisterClientAsync(request.Name, _clientAddress);

            if (_connectionService.Get(request.Id) is not null)
            {
                await _loggerService.SendLogAsync(_logger,
                    request.Id,
                    $"{client.Name} [{_clientAddress}] wanted to connect, but it's already on list!",
                    LogLevel.Information);
                break;
            }

            await _connectionService.RegisterAsync(request.Id, client.Id, responseStream);
        }

        await _connectionService.CloseAsync(_connectionGuid);
    }

    private async Task HandleExceptionAsync(Exception ex, ServerCallContext context)
    {
        if (context.CancellationToken.IsCancellationRequested && _connectionGuid is not null)
        {
            await _connectionService.CloseAsync(_connectionGuid);
            return;
        }

        _logger.LogError($"[{_clientAddress}] - {ex.Message}");
    }
}

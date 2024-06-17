using Grpc.Core;
using Server.Protos;
using Server.Services.Interfaces;
using Server.Utilities.Exceptions;

namespace Server.Services;

internal class HandlerService(
    ILogger<HandlerService> logger,
    ILoggerService loggerService,
    IClientService clientService
    ) : Handler.HandlerBase
{
    private readonly ILogger<HandlerService> _logger = logger;
    private readonly IClientService _clientService = clientService;
    private readonly ILoggerService _loggerService = loggerService;
    private string _clientAddress = null!;

    public override async Task CommandStream(IAsyncStreamReader<InitRequest> requestStream, IServerStreamWriter<CommandReply> responseStream, ServerCallContext context)
    {
        _clientAddress = context.Host;

        try
        {
            await HandleClientAsync(requestStream, responseStream);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(ex, context);
        }
    }

    private async Task HandleClientAsync(IAsyncStreamReader<InitRequest> requestStream, IServerStreamWriter<CommandReply> responseStream)
    {
        bool isFirstRequest = true;

        await foreach (var request in requestStream.ReadAllAsync())
        {
            if (!isFirstRequest)
            {
                await _loggerService.LogAndSendMessage(_logger, request.Id, request.Message, LogLevel.Information);
                continue;
            }

            var existingClient = _clientService.GetClientByIp(_clientAddress);

            if (existingClient != null)
            {
                await _loggerService.LogAndSendMessage(_logger, existingClient.Id, $"Client {existingClient.Name} [{existingClient.AddressIp}] wanted to connect, but it's already on list!", LogLevel.Information);
                break;
            }

            await _clientService.RegisterClientAsync(responseStream, request.Id, request.Name, _clientAddress);
            isFirstRequest = false;
        }

        await _clientService.RemoveClientByIpAsync(_clientAddress, "connection finished");
    }

    private async Task HandleExceptionAsync(Exception ex, ServerCallContext context)
    {
        if (context.CancellationToken.IsCancellationRequested)
        {
            await _clientService.RemoveClientByIpAsync(_clientAddress, "due to cancellation");
        }
        else
        {
            _logger.LogErrorWithTimestamp($"[{_clientAddress}] - {ex.Message}");
        }
    }
}

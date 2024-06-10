using Grpc.Core;
using Server.Models;
using Server.Protos;
using Server.Services.Interfaces;
using Server.Utilities.Exceptions;
using System.Collections.Concurrent;

namespace Server.Services;

public class HandlerService(ILogger<HandlerService> logger, ILoggerService loggerService, IHandlerHelper handlerHelper) : Handler.HandlerBase
{
    private readonly ILogger<HandlerService> _logger = logger;
    private readonly IHandlerHelper _handlerHelper = handlerHelper;
    private readonly ILoggerService _loggerService = loggerService;

    public readonly ConcurrentQueue<byte[]> frames = new();
    public readonly ConcurrentDictionary<ClientModel, IServerStreamWriter<CommandReply>> connectedClients = new();

    public override async Task CommandStream(IAsyncStreamReader<InitRequest> requestStream, IServerStreamWriter<CommandReply> responseStream, ServerCallContext context)
    {
        string clientAddress = context.Host;

        try
        {
            await HandleClientAsync(clientAddress, requestStream, responseStream);
        }
        catch (Exception ex)
        {
            await HandleException(ex, clientAddress, context);
        }
    }

    public override async Task StreamDesktop(IAsyncStreamReader<DesktopFrame> requestStream, IServerStreamWriter<Empty> responseStream, ServerCallContext context)
    {
        await foreach (var frame in requestStream.ReadAllAsync())
        {
            frames.Enqueue(frame.Image.ToByteArray());
        }
    }

    private async Task HandleClientAsync(string clientAddress, IAsyncStreamReader<InitRequest> requestStream, IServerStreamWriter<CommandReply> responseStream)
    {
        bool isFirstRequest = true;

        await foreach (var request in requestStream.ReadAllAsync())
        {
            if (isFirstRequest)
            {
                var existingClient = _handlerHelper.GetClientByIp(connectedClients, clientAddress);

                if (existingClient == null)
                {
                    var currentClient = new ClientModel
                    {
                        Id = request.Id,
                        Name = request.Name,
                        AddressIp = clientAddress,
                        LastConnectionDate = DateTime.UtcNow
                    };

                    if (connectedClients.TryAdd(currentClient, responseStream))
                    {
                        await _loggerService.LogAndSendMessage(_logger, currentClient.Id, $"Client {currentClient.Name} [{currentClient.AddressIp}] connected successfully!", LogLevel.Information);
                        isFirstRequest = false;
                    }
                    else
                    {
                        _logger.LogErrorWithTimestamp($"Failed adding {currentClient.Name} [{currentClient.AddressIp}] to list.");
                    }
                }
                else
                {
                    await _loggerService.LogAndSendMessage(_logger, existingClient.Id, $"Client {existingClient.Name} [{existingClient.AddressIp}] wanted to connect, but it's already on list!", LogLevel.Information);
                }
            }

            await _loggerService.LogAndSendMessage(_logger, request.Id, request.Message, LogLevel.Information);
        }

        await _handlerHelper.RemoveClientByIpAsync(connectedClients, clientAddress, "connection finished");
    }

    private async Task HandleException(Exception ex, string clientAddress, ServerCallContext context)
    {
        if (context.CancellationToken.IsCancellationRequested)
        {
            await _handlerHelper.RemoveClientByIpAsync(connectedClients, clientAddress, "due to cancellation");
        }
        else
        {
            _logger.LogErrorWithTimestamp($"[{clientAddress}] - {ex.Message}");
        }
    }
}

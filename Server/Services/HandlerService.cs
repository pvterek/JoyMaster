using Grpc.Core;
using Server.Exceptions;
using Server.Models;
using Server.Protos;
using System.Collections.Concurrent;
using static Server.Services.HandlerHelper; //get rid of it

namespace Server.Services;

public class HandlerService(ILogger<HandlerService> logger, ConsoleService consoleService) : Handler.HandlerBase
{
    private readonly ILogger<HandlerService> _logger = logger;
    private readonly LoggerService _loggerService = new(logger, consoleService);

    public static readonly ConcurrentQueue<byte[]> Frames = new();
    public static readonly ConcurrentDictionary<ClientModel, IServerStreamWriter<CommandReply>> ConnectedClients = new();

    public override async Task CommandStream(IAsyncStreamReader<InitRequest> requestStream, IServerStreamWriter<CommandReply> responseStream, ServerCallContext context)
    {
        string clientAddress = context.Host;

        try
        {
            await HandleClientAsync(clientAddress, requestStream, responseStream);
        }
        catch (Exception ex)
        {
            HandleException(ex, clientAddress, context);
        }
    }

    public override async Task StreamDesktop(IAsyncStreamReader<DesktopFrame> requestStream, IServerStreamWriter<Empty> responseStream, ServerCallContext context)
    {
        await foreach (var frame in requestStream.ReadAllAsync())
        {
            Frames.Enqueue(frame.Image.ToByteArray());
        }
    }

    private async Task HandleClientAsync(string clientAddress, IAsyncStreamReader<InitRequest> requestStream, IServerStreamWriter<CommandReply> responseStream)
    {
        bool isFirstRequest = true;

        await foreach (var request in requestStream.ReadAllAsync())
        {
            if (isFirstRequest)
            {
                var existingClient = GetClientByIp(clientAddress);

                if (existingClient == null)
                {
                    var currentClient = new ClientModel
                    {
                        Id = request.Id,
                        Name = request.Name,
                        AddressIp = clientAddress,
                        LastConnectionDate = DateTime.UtcNow
                    };

                    if (ConnectedClients.TryAdd(currentClient, responseStream))
                    {
                        _loggerService.LogAndSendMessage(currentClient.Id, $"Client {currentClient.Name} [{currentClient.AddressIp}] connected successfully!", LogLevel.Information);
                        isFirstRequest = false;
                    }
                    else
                    {
                        _logger.LogErrorWithTimestamp($"Failed adding {currentClient.Name} [{currentClient.AddressIp}] to list.");
                    }
                }
                else
                {
                    _loggerService.LogAndSendMessage(existingClient.Id, $"Client {existingClient.Name} [{existingClient.AddressIp}] wanted to connect, but it's already on list!", LogLevel.Information);
                }
            }

            _loggerService.LogAndSendMessage(request.Id, request.Message, LogLevel.Information);
        }

        RemoveClientByIp(clientAddress, "connection finished");
    }

    private void HandleException(Exception ex, string clientAddress, ServerCallContext context)
    {
        if (context.CancellationToken.IsCancellationRequested)
        {
            RemoveClientByIp(clientAddress, "due to cancellation");
        }
        else
        {
            _logger.LogErrorWithTimestamp($"[{clientAddress}] - {ex.Message}");
        }
    }

    private void RemoveClientByIp(string clientAddress, string reason) //move to helper class
    {
        var client = GetClientByIp(clientAddress);

        if (client == null)
        {
            _loggerService.LogAndSendMessage("System", $"No client found with IP address: {clientAddress}", LogLevel.Warning);
        }
        else
        {
            if (ConnectedClients.TryRemove(client, out _))
            {
                _loggerService.LogAndSendMessage(client.Id, $"Client {clientAddress} disconnected: {reason}", LogLevel.Information);
            }
            else
            {
                _loggerService.LogAndSendMessage("System", $"Failed to remove client with IP address: {clientAddress}", LogLevel.Warning);
            }
        }
    }
}

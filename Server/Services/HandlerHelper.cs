using Grpc.Core;
using Server.Models;
using Server.Protos;
using Server.Services.Interfaces;
using System.Collections.Concurrent;

namespace Server.Services;

public class HandlerHelper(ILogger<HandlerHelper> logger, ILoggerService loggerService) : IHandlerHelper
{
    private readonly ILogger<HandlerHelper> _logger = logger;
    private readonly ILoggerService _loggerService = loggerService;

    public ClientModel? GetClientByIp(ConcurrentDictionary<ClientModel, IServerStreamWriter<CommandReply>> clientsList, string clientIpAddress)
    {
        return clientsList.Keys.SingleOrDefault(client => client.AddressIp == clientIpAddress);
    }

    public async Task RemoveClientByIpAsync(ConcurrentDictionary<ClientModel, IServerStreamWriter<CommandReply>> clientsList, string clientIpAddress, string reason)
    {
        var client = GetClientByIp(clientsList, clientIpAddress);

        if (client == null)
        {
            await _loggerService.LogAndSendMessage(_logger, "System", $"No client found with IP address: {clientIpAddress}", LogLevel.Warning);
        }
        else
        {
            if (clientsList.TryRemove(client, out _))
            {
                await _loggerService.LogAndSendMessage(_logger, client.Id, $"Client {clientIpAddress} disconnected: {reason}", LogLevel.Information);
            }
            else
            {
                await _loggerService.LogAndSendMessage(_logger, client.Id, $"Failed to remove client with IP address: {clientIpAddress}", LogLevel.Warning);
            }
        }
    }
}

using Grpc.Core;
using Server.Models;
using Server.Protos;
using Server.Services.Interfaces;
using System.Collections.Concurrent;

namespace Server.Services;

public class HandlerHelper : IHandlerHelper
{
    public ClientModel? GetClientByIp(ConcurrentDictionary<ClientModel, IServerStreamWriter<CommandReply>> clientsList, string clientIpAddress)
    {
        return clientsList.Keys.SingleOrDefault(client => client.AddressIp == clientIpAddress);
    }

    public void RemoveClientByIp(ConcurrentDictionary<ClientModel, IServerStreamWriter<CommandReply>> clientsList, string clientIpAddress, string reason, LoggerService logger)
    {
        var client = GetClientByIp(clientsList, clientIpAddress);

        if (client == null)
        {
            logger.LogAndSendMessage("System", $"No client found with IP address: {clientIpAddress}", LogLevel.Warning);
        }
        else
        {
            if (clientsList.TryRemove(client, out _))
            {
                logger.LogAndSendMessage(client.Id, $"Client {clientIpAddress} disconnected: {reason}", LogLevel.Information);
            }
            else
            {
                logger.LogAndSendMessage("System", $"Failed to remove client with IP address: {clientIpAddress}", LogLevel.Warning);
            }
        }
    }
}

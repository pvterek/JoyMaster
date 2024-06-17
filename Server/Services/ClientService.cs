using Grpc.Core;
using Server.Models;
using Server.Protos;
using Server.Services.Interfaces;
using Server.Utilities.Exceptions;

namespace Server.Services;

internal class ClientService(
    ILogger<ClientService> logger,
    ILoggerService loggerService,
    IClientDictionary clientDictionary
    ) : IClientService
{
    private readonly ILogger<ClientService> _logger = logger;
    private readonly ILoggerService _loggerService = loggerService;
    private readonly IClientDictionary _clientDictionary = clientDictionary;

    public ClientModel? GetClientByIp(string clientIpAddress)
    {
        return _clientDictionary.Clients.Keys.SingleOrDefault(client => client.AddressIp == clientIpAddress);
    }

    public async Task RemoveClientByIpAsync(string clientIpAddress, string reason)
    {
        var client = GetClientByIp(clientIpAddress);

        if (client == null)
        {
            await _loggerService.LogAndSendMessage(_logger, "System", $"No client found with IP address: {clientIpAddress}", LogLevel.Warning);
            return;
        }

        if (_clientDictionary.Clients.TryRemove(client, out _))
        {
            await _loggerService.LogAndSendMessage(_logger, client.Id, $"Client {clientIpAddress} disconnected: {reason}", LogLevel.Information);
            return;
        }

        await _loggerService.LogAndSendMessage(_logger, client.Id, $"Failed to remove client with IP address: {clientIpAddress}", LogLevel.Warning);
    }

    public async Task RegisterClientAsync(IServerStreamWriter<CommandReply> responseStream, string clientId, string clientName, string clientAddress)
    {
        var currentClient = CreateClientModel(clientId, clientName, clientAddress);

        if (_clientDictionary.Clients.TryAdd(currentClient, responseStream))
        {
            await _loggerService.LogAndSendMessage(_logger, currentClient.Id, $"Client {currentClient.Name} [{currentClient.AddressIp}] connected successfully!", LogLevel.Information);
            return;
        }

        _logger.LogErrorWithTimestamp($"Failed adding {currentClient.Name} [{currentClient.AddressIp}] to list.");
    }

    private ClientModel CreateClientModel(string clientId, string clientName, string clientAddress)
    {
        return new ClientModel
        {
            Id = clientId,
            Name = clientName,
            AddressIp = clientAddress,
            LastConnectionDate = DateTime.UtcNow
        };
    }
}

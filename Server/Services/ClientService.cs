using Server.Models;
using Server.Repository;

namespace Server.Services;

public class ClientService(
    IClientRepository clientRepository,
    ILogger<ClientService> logger)
{
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly ILogger<ClientService> _logger = logger;

    public async Task<Client?> GetClientAsync(string clientName, string clientIpAddress)
    {
        return await _clientRepository.GetAsync(clientName, clientIpAddress);
    }

    public async Task<Client> RegisterClientAsync(string clientName, string clientIpAddress)
    {
        var client = CreateClientEntity(clientName, clientIpAddress);

        await _clientRepository.AddAsync(client);

        return client;
    }

    private Client CreateClientEntity(string name, string ipAddress)
    {
        return new Client
        {
            Name = name,
            IpAddress = ipAddress
        };
    }
}

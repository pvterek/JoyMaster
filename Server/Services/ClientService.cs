﻿using Server.Entities;
using Server.Models;
using Server.Repository.Interfaces;

namespace Server.Services;

public class ClientService(
    IClientRepository clientRepository,
    EntitiesCreator entitiesCreator)
{
    private readonly IClientRepository _clientRepository = clientRepository;
    private readonly EntitiesCreator _entitiesCreator = entitiesCreator;

    public async Task<Client?> GetClientAsync(string clientName, string clientIpAddress)
    {
        return await _clientRepository.GetAsync(clientName, clientIpAddress);
    }

    public async Task<Client> RegisterClientAsync(string clientName, string clientIpAddress)
    {
        var client = _entitiesCreator.CreateClient(clientName, clientIpAddress);
        await _clientRepository.AddAsync(client);
        return client;
    }
}

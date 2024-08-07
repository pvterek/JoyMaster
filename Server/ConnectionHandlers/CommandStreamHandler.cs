﻿using Grpc.Core;
using Server.Models;
using Server.Protos;
using Server.Services;
using Server.Services.Interfaces;
using Server.Utilities.Logs;

namespace Server.ConnectionsHandlers;

public class CommandStreamHandler(
    ILogger<CommandStreamHandler> logger,
    IConnectionService connectionService,
    LoggerService loggerService,
    ClientService clientService
    ) : CommandStreamer.CommandStreamerBase
{
    private readonly ILogger<CommandStreamHandler> _logger = logger;
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
                await _loggerService.SendLogAsync(
                    _logger,
                    request.Id,
                    request.Message,
                    LogLevel.Information);
                continue;
            }

            await HandleInitialRequestAsync(request, responseStream);
        }

        await _connectionService.CloseAsync(_connectionGuid);
    }

    private async Task HandleInitialRequestAsync(
        Request request,
        IServerStreamWriter<Response> responseStream)
    {
        var client = await GetOrRegisterClientAsync(request.Name, _clientAddress);

        if (_connectionService.GetActive(request.Id).Key is not null)
        {
            await _loggerService.SendLogAsync(
                _logger,
                request.Id,
                $"{client.Name} [{_clientAddress}] wanted to connect, but it's already on the list!",
                LogLevel.Information);
            return;
        }

        await _connectionService.RegisterAsync(request.Id, client.Id, responseStream);
    }

    private async Task<Client> GetOrRegisterClientAsync(string clientName, string clientAddress)
    {
        var existingClient = await _clientService.GetClientAsync(clientName, clientAddress);
        if (existingClient != null)
        {
            return existingClient;
        }
        else
        {
            return await _clientService.RegisterClientAsync(clientName, clientAddress);
        }
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

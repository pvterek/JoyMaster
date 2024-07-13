using Grpc.Core;
using Server.CommandHandlers;
using Server.Entities;
using Server.Models;
using Server.Protos;
using Server.Repository.Interfaces;
using Server.Services.Interfaces;
using Server.Utilities;
using Server.Utilities.Logs;

namespace Server.Services;

public class ConnectionService(
    ILogger<ConnectionService> logger,
    LoggerService loggerService,
    CommandHandlerRegistry commandHandlerRegistry,
    IActiveConnections activeConnections,
    IConnectionRepository connectionRepository,
    EntitiesCreator entitiesCreator
    ) : IConnectionService
{
    private readonly ILogger<ConnectionService> _logger = logger;
    private readonly LoggerService _loggerService = loggerService;
    private readonly CommandHandlerRegistry _commandHandlerRegistry = commandHandlerRegistry;
    private readonly IActiveConnections _activeConnections = activeConnections;
    private readonly IConnectionRepository _connectionRepository = connectionRepository;
    private readonly EntitiesCreator _entitiesCreator = entitiesCreator;

    public async Task RegisterAsync(
        string connectionGuid,
        int clientId,
        IServerStreamWriter<Response> responseStream)
    {
        var connection = _entitiesCreator.CreateConnection(connectionGuid, clientId);

        if (_activeConnections.Connections.TryAdd(connection, responseStream))
        {
            await _connectionRepository.AddAsync(connection);

            await _loggerService.SendLogAsync(
                _logger,
                connection.ConnectionGuid,
                "Successful connection!",
                LogLevel.Information);
            return;
        }

        _logger.LogError($"Failed registering {connection.ConnectionGuid} request.");
    }

    public async Task CloseAsync(string connectionGuid)
    {
        var connection = GetActive(connectionGuid);

        if (connection.Key is null)
        {
            _logger.LogError($"Null: {connectionGuid}.");
            return;
        }

        await SetDisconnectedTime(connection.Key);

        var handler = _commandHandlerRegistry.Resolve(AppConstants.EndCommand);
        await handler.ExecuteAsync(connectionGuid);

        _activeConnections.Connections.TryRemove(connection);

        await _loggerService.SendLogAsync(
                _logger,
                connectionGuid,
                "Connection closed successfully!",
                LogLevel.Information);
    }

    public KeyValuePair<Connection, IServerStreamWriter<Response>> GetActive(string connectionGuid)
    {
        return _activeConnections
            .Connections
            .FirstOrDefault(pair => pair.Key.ConnectionGuid == connectionGuid);
    }

    public List<int> GetIdsList()
    {
        return _activeConnections
            .Connections
            .Keys
            .Select(c => c.Id).ToList();
    }

    public bool ConnectionExists(string connectionGuid)
    {
        return _activeConnections
            .Connections
            .Keys
            .Any(c => c.ConnectionGuid == connectionGuid);
    }

    private async Task SetDisconnectedTime(Connection connection)
    {
        connection.DisconnectedTime = DateTime.UtcNow;
        await _connectionRepository.UpdateAsync(connection);
    }
}

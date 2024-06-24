using Grpc.Core;
using Server.Models;
using Server.Protos;
using Server.Repository;
using Server.Services.Interfaces;
using Server.Utilities.Logs;

namespace Server.Services;

internal class ConnectionService(
    ILogger<ConnectionService> logger,
    LoggerService loggerService,
    IActiveConnections activeConnections,
    IConnectionRepository connectionRepository
    ) : IConnectionService
{
    private readonly ILogger<ConnectionService> _logger = logger;
    private readonly LoggerService _loggerService = loggerService;
    private readonly IActiveConnections _activeConnections = activeConnections;
    private readonly IConnectionRepository _connectionRepository = connectionRepository;

    public async Task RegisterAsync(
        string connectionGuid,
        int clientId,
        IServerStreamWriter<Response> responseStream)
    {
        var connection = CreateConnectionEntity(connectionGuid, clientId);

        if (_activeConnections.Connections.TryAdd(connection, responseStream))
        {
            await _connectionRepository.AddAsync(connection);

            await _loggerService.SendLogAsync(
                _logger,
                connectionGuid,
                "Successful connection!",
                LogLevel.Information);
            return;
        }

        _logger.LogError($"Failed registering {connectionGuid} request.");
    }

    public async Task CloseAsync(string connectionGuid)
    {
        var connection = Get(connectionGuid);

        if (connection is null)
        {
            _logger.LogError($"Null: {connectionGuid}.");
            return;
        }

        if (_activeConnections.Connections.TryRemove(connection, out _))
        {
            await SetDisconnectedTime(connection);

            await _loggerService.SendLogAsync(
                _logger,
                connectionGuid,
                "Connection closed successfully!",
                LogLevel.Information);
            return;
        }

        await _loggerService.SendLogAsync(
                _logger,
                connectionGuid,
                "Closing connection failed!",
                LogLevel.Error);
    }

    public Connection? Get(string connectionGuid)
    {
        return _activeConnections.Connections.Keys
            .FirstOrDefault(c => c.ConnectionGuid == connectionGuid);
    }

    private Connection CreateConnectionEntity(string connectionGuid, int clientId)
    {
        return new Connection
        {
            ConnectionGuid = connectionGuid,
            ClientId = clientId
        };
    }

    private async Task SetDisconnectedTime(Connection connection)
    {
        connection.DisconnectedTime = DateTime.UtcNow;
        await _connectionRepository.UpdateAsync(connection);
    }
}

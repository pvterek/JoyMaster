using Server.Protos;
using Server.Services.Interfaces;
using Server.Utilities.Logs;

namespace Server.Services;

public class CommandSender(
    ILogger<CommandSender> logger,
    LoggerService loggerService,
    IConnectionService connectionService)
{
    private readonly ILogger<CommandSender> _logger = logger;
    private readonly LoggerService _loggerService = loggerService;
    private readonly IConnectionService _connectionService = connectionService;

    public async Task SendCommand(string connectionGuid, Response response)
    {
        var existingConnection = _connectionService.GetActive(connectionGuid);
        if (existingConnection.Key is null)
        {
            await _loggerService.SendLogAsync(
                _logger,
                connectionGuid,
                $"No active connection found for: {connectionGuid}!",
                LogLevel.Warning);
            return;
        }

        await existingConnection.Value.WriteAsync(response);

        await _loggerService.SendLogAsync(
            _logger,
            connectionGuid,
            $"Command sent!",
            LogLevel.Information);
    }
}

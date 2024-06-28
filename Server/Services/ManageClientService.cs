using Server.Models;
using Server.Protos;
using Server.Services.Interfaces;
using Server.Utilities.Constants;
using Server.Utilities.Logs;

namespace Server.Services;

public class ManageClientService(
    ILogger<ManageClientService> logger,
    LoggerService loggerService,
    IActiveConnections activeConnections
    )
{
    private readonly ILogger<ManageClientService> _logger = logger;
    private readonly LoggerService _loggerService = loggerService;
    private readonly IActiveConnections _activeConnections = activeConnections;

    public async Task ProcessCommand(Message commandModel)
    {
        if (string.IsNullOrEmpty(commandModel.MessageContent))
        {
            await _loggerService.SendLogAsync(
                _logger,
                commandModel.ConnectionGuid,
                "Provided empty command!",
                LogLevel.Warning);
            return;
        }

        var (command, parameters) = ParseCommand(commandModel.MessageContent);

        var response = CreateResponseEntity(command, parameters);

        await ExecuteCommandAsync(commandModel.ConnectionGuid, response);
    }

    private (string command, string parameters) ParseCommand(string message)
    {
        var firstSpaceIndex = message.IndexOf(' ');
        if (firstSpaceIndex == -1)
            return (message, string.Empty);

        var command = message[..firstSpaceIndex];
        var parameters = message[(firstSpaceIndex + 1)..];
        return (command, parameters);
    }

    private async Task ExecuteCommandAsync(string connectionGuid, Response response)
    {
        var command = response.Command;

        switch (command)
        {
            case AppConstants.EndCommand:
            case AppConstants.AlertCommand:
            case AppConstants.SendCommand:
            case AppConstants.StreamCommand:
                await SendCommand(connectionGuid, response);
                break;

            default:
                await _loggerService.SendLogAsync(
                    _logger,
                    connectionGuid,
                    $"Invalid command: {command}",
                    LogLevel.Warning);
                break;
        }
    }

    private async Task SendCommand(string connectionGuid, Response response)
    {
        var existingConnection = _activeConnections.GetActiveConnection(connectionGuid);
        if (existingConnection.Key is null)
        {
            await _loggerService.SendLogAsync(
                _logger,
                connectionGuid,
                $"No active connection found for: {connectionGuid}!",
                LogLevel.Warning);
        }

        await existingConnection.Value.WriteAsync(response);

        await _loggerService.SendLogAsync(
            _logger,
            connectionGuid,
            $"Command sent!",
            LogLevel.Information);
    }

    private Response CreateResponseEntity(string command, string parameters)
    {
        return new Response()
        {
            Command = command.ToLower(),
            Parameters = parameters
        };
    }
}

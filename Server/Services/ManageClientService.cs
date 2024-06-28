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

        string command;
        string parameters = string.Empty;
        string message = commandModel.MessageContent;
        int firstSpaceIndex = message.IndexOf(' ');

        if (firstSpaceIndex == -1)
        {
            command = message;
        }
        else
        {
            command = message[..firstSpaceIndex];
            parameters = message[(firstSpaceIndex + 1)..];
        }

        switch (command.ToLower())
        {
            case AppConstants.EndCommand:
                await SendCommand(commandModel.ConnectionGuid, command);
                break;

            case AppConstants.SendCommand:
                await SendCommand(commandModel.ConnectionGuid, parameters);
                break;

            case AppConstants.StreamCommand:
                await SendCommand(commandModel.ConnectionGuid, "stream enable");
                break;

            case AppConstants.AlertCommand:
                await SendCommand(commandModel.ConnectionGuid, command);
                break;

            default:
                await _loggerService.SendLogAsync(
                    _logger,
                    commandModel.ConnectionGuid,
                    $"Invalid command: {commandModel.MessageContent}",
                    LogLevel.Warning);
                break;
        }
    }

    private async Task SendCommand(string connectionGuid, string parameters)
    {
        var existingConnection = _activeConnections.Connections.FirstOrDefault(pair => pair.Key.ConnectionGuid == connectionGuid);
        if (existingConnection.Key == null)
        {
            await _loggerService.SendLogAsync(
                _logger,
                connectionGuid,
                $"No client found for: {connectionGuid}!",
                LogLevel.Warning);
        }

        var response = new Response
        {
            Message = parameters
        };

        await existingConnection.Value.WriteAsync(response);

        await _loggerService.SendLogAsync(
            _logger,
            connectionGuid,
            $"Command '{parameters}' sent!.",
            LogLevel.Information);
    }
}

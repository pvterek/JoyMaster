using Server.Models;
using Server.Protos;
using Server.Services.Interfaces;
using Server.Utilities.Constants;
using Server.Utilities.Logs;

namespace Server.Services;

public class ManageClientService(
    ILogger<ManageClientService> logger,
    LoggerService loggerService,
    IClientDictionary clientDictionary
    )
{
    private readonly ILogger<ManageClientService> _logger = logger;
    private readonly LoggerService _loggerService = loggerService;
    private readonly IClientDictionary _clientDictionary = clientDictionary;

    public async Task ProcessCommand(MessageModel commandModel)
    {
        if (string.IsNullOrEmpty(commandModel.Message))
        {
            await _loggerService.SendMessageWithLogAsync(_logger, commandModel.ClientId, "Provided empty command!", LogLevel.Warning);
            return;
        }

        string command;
        string parameters = string.Empty;
        string message = commandModel.Message;
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
                await SendCommand(commandModel.ClientId, command);
                break;
            case AppConstants.SendCommand:
                await SendCommand(commandModel.ClientId, parameters);
                break;
            default:
                await _loggerService.SendMessageWithLogAsync(_logger, commandModel.ClientId, $"Invalid command: {commandModel.Message}", LogLevel.Warning);
                break;
        }
    }

    private async Task SendCommand(string clientId, string parameters)
    {
        var existingClient = _clientDictionary.Clients.FirstOrDefault(pair => pair.Key.Id == clientId);
        if (existingClient.Key == null)
        {
            await _loggerService.SendMessageWithLogAsync(_logger, clientId, $"No client found for: {clientId}", LogLevel.Warning);
        }

        var response = new CommandReply
        {
            Message = parameters
        };

        await existingClient.Value.WriteAsync(response);

        await _loggerService.SendMessageWithLogAsync(_logger, clientId, $"Command '{parameters}' sent to client {clientId}.", LogLevel.Information);
    }
}

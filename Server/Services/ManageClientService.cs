using Server.Models;
using Server.Protos;
using static Server.Services.HandlerService; //get rid of it

namespace Server.Services;

public class ManageClientService(ILogger<ManageClientService> logger, ConsoleService consoleService)
{
    private readonly LoggerService _loggerService = new(logger, consoleService);

    //move to separate class
    private const string SendCommandPrompt = "send";
    private const string EndCommandPrompt = "end";

    public async Task ProcessCommand(CommandModel commandModel)
    {
        if (string.IsNullOrEmpty(commandModel.Command))
        {
            _loggerService.LogAndSendMessage(commandModel.ClientId, "Provided empty command!", LogLevel.Warning);
            return;
        }

        string command;
        string parameters = string.Empty;
        string message = commandModel.Command;
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
            case EndCommandPrompt:
                await SendCommand(commandModel.ClientId, command);
                break;
            case SendCommandPrompt:
                await SendCommand(commandModel.ClientId, parameters);
                break;
            default:
                _loggerService.LogAndSendMessage(commandModel.ClientId, $"Invalid command: {commandModel.Command}", LogLevel.Warning);
                break;
        }
    }

    private async Task SendCommand(string clientId, string parameters)
    {
        var existingClient = ConnectedClients.FirstOrDefault(pair => pair.Key.Id == clientId);
        if (existingClient.Key == null)
        {
            _loggerService.LogAndSendMessage(clientId, $"No client found for: {clientId}", LogLevel.Warning);
        }

        var response = new CommandReply
        {
            Message = parameters
        };

        await existingClient.Value.WriteAsync(response);

        _loggerService.LogAndSendMessage(clientId, $"Command '{parameters}' sent to client {clientId}.", LogLevel.Information);
    }
}

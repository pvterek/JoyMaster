using Server.CommandHandlers;
using Server.Entities;
using Server.Models;
using Server.Protos;
using Server.Services.Interfaces;
using Server.Utilities.Logs;

namespace Server.Services;

public class CommandExecutor(
    ILogger<CommandExecutor> logger,
    LoggerService loggerService,
    CommandHandlerRegistry commandHandlerRegistry,
    EntitiesCreator entitiesCreator
    ) : ICommandExecutor
{
    private readonly ILogger<CommandExecutor> _logger = logger;
    private readonly LoggerService _loggerService = loggerService;
    private readonly CommandHandlerRegistry _commandHandlerRegistry = commandHandlerRegistry;
    private readonly EntitiesCreator _entitiesCreator = entitiesCreator;

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

        var response = _entitiesCreator.CreateResponse(command, parameters);

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
        var command = response.Command.ToLower();
        var handler = _commandHandlerRegistry.Resolve(command);

        if (handler == null)
        {
            await _loggerService.SendLogAsync(
                _logger,
                connectionGuid,
                $"Invalid command: {command}",
                LogLevel.Warning);

            return;
        }

        await handler.ExecuteAsync(connectionGuid, response);
    }
}

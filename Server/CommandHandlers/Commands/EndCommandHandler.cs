using Server.CommandHandlers.Interfaces;
using Server.Entities;
using Server.Protos;
using Server.Services;
using Server.Utilities;

namespace Server.CommandHandlers.Commands;

public class EndCommandHandler(
    EntitiesCreator entitiesCreator,
    CommandSender commandSender
    ) : ICommandHandler
{
    private readonly EntitiesCreator _entitiesCreator = entitiesCreator;
    private readonly CommandSender _commandSender = commandSender;

    public async Task ExecuteAsync(string connectionGuid, Response? response = null)
    {
        var endCommand = _entitiesCreator.CreateResponse(AppConstants.EndCommand, string.Empty);
        await _commandSender.SendCommand(connectionGuid, endCommand);
    }
}

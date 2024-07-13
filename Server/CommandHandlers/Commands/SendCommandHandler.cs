using Server.CommandHandlers.Interfaces;
using Server.Protos;
using Server.Services;

namespace Server.CommandHandlers.Commands;

public class SendCommandHandler(CommandSender commandSender) : ICommandHandler
{
    private readonly CommandSender _commandSender = commandSender;

    public async Task ExecuteAsync(string connectionGuid, Response? response)
    {
        ArgumentNullException.ThrowIfNull(response);
        await _commandSender.SendCommand(connectionGuid, response);
    }
}

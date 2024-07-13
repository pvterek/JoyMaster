using Server.Interfaces;
using Server.Protos;
using Server.Services;

namespace Server.CommandHandlers;

internal class StreamCommandHandler(CommandSender commandSender) : ICommandHandler
{
    private readonly CommandSender _commandSender = commandSender;

    public async Task ExecuteAsync(string connectionGuid, Response? response)
    {
        ArgumentNullException.ThrowIfNull(response);
        await _commandSender.SendCommand(connectionGuid, response);
    }
}

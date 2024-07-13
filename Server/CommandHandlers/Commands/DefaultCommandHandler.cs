using Server.CommandHandlers.Interfaces;
using Server.Protos;

namespace Server.CommandHandlers.Commands;

public class DefaultCommandHandler : ICommandHandler
{
    public Task ExecuteAsync(string connectionGuid, Response? response = null)
    {
        return Task.CompletedTask;
    }
}

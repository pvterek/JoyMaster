using Server.Interfaces;
using Server.Protos;

namespace Server.CommandHandlers;

public class DefaultCommandHandler : ICommandHandler
{
    public Task ExecuteAsync(string connectionGuid, Response? response = null)
    {
        return Task.CompletedTask;
    }
}

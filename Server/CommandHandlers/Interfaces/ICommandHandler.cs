using Server.Protos;

namespace Server.CommandHandlers.Interfaces;

public interface ICommandHandler
{
    Task ExecuteAsync(string connectionGuid, Response? response = null);
}

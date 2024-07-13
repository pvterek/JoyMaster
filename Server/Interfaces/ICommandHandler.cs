using Server.Protos;

namespace Server.Interfaces;

public interface ICommandHandler
{
    Task ExecuteAsync(string connectionGuid, Response? response = null);
}

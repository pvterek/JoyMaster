using Server.Models;

namespace Server.Services.Interfaces;

public interface ICommandExecutor
{
    Task ProcessCommand(Message commandModel);
}

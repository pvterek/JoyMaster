using Server.Interfaces;

namespace Server.CommandHandlers;

public class CommandHandlerRegistry()
{
    private readonly Dictionary<string, Func<ICommandHandler>> _handlers = [];

    public void Register(string command, Func<ICommandHandler> handlerFactory)
    {
        _handlers[command] = handlerFactory;
    }

    public ICommandHandler Resolve(string command)
    {
        if (_handlers.TryGetValue(command, out var handlerFactory))
        {
            return handlerFactory();
        }

        throw new InvalidOperationException($"No handler registered for command {command}");
    }
}

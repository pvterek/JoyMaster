using Server.Interfaces;

namespace Server.CommandHandlers;

public class CommandHandlerRegistry()
{
    private readonly Dictionary<string, Func<ICommandHandler>> _handlers = [];

    public void Register(string command, Func<ICommandHandler> handler)
    {
        _handlers[command] = handler;
    }

    public ICommandHandler Resolve(string command)
    {
        if (_handlers.TryGetValue(command, out var handler))
        {
            return handler();
        }

        throw new InvalidOperationException($"No handler registered for command {command}");
    }
}

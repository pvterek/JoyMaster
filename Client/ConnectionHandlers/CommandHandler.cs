﻿using Client.CommandHandlers;
using Grpc.Core;
using Server.Protos;

namespace Client.ConnectionHandlers;

public class CommandHandler(CommandHandlerRegistry commandHandlerRegistry)
{
    private readonly CommandHandlerRegistry _commandHandlerRegistry = commandHandlerRegistry;

    public async Task HandleCommandAsync(Response response, IClientStreamWriter<Request> requestStream)
    {
        var command = response.Command.ToLower();
        var handler = _commandHandlerRegistry.Resolve(command);

        ArgumentNullException.ThrowIfNull(handler);

        await handler.ExecuteAsync(response, requestStream);
    }
}

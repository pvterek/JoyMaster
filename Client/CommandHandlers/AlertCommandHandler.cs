﻿using Grpc.Core;
using Server.Protos;
using System.Windows;

namespace Client.CommandHandlers;

internal class AlertCommandHandler : ICommandHandler
{
    public async Task ExecuteAsync(
        Response? response,
        IClientStreamWriter<Request>? requestStream)
    {
        ArgumentNullException.ThrowIfNull(response);
        MessageBox.Show(response.Parameters, "JoyMaster", MessageBoxButton.OK);

        await Task.CompletedTask;
    }
}
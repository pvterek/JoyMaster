using Client.CommandHandlers.Interfaces;
using Client.Protos;
using Grpc.Core;
using System.Windows;

namespace Client.CommandHandlers.Commands;

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

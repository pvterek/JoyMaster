using Client.Protos;
using Grpc.Core;

namespace Client.CommandHandlers.Interfaces;

public interface ICommandHandler
{
    Task ExecuteAsync(Response? response = null, IClientStreamWriter<Request>? requestStream = null);
}

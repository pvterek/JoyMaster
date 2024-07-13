using Grpc.Core;
using Server.Protos;

namespace Client.CommandHandlers.Interfaces;

public interface ICommandHandler
{
    Task ExecuteAsync(Response? response = null, IClientStreamWriter<Request>? requestStream = null);
}

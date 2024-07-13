using Grpc.Core;
using Server.Protos;

namespace Client.CommandHandlers;

public interface ICommandHandler
{
    Task ExecuteAsync(Response? response = null, IClientStreamWriter<Request>? requestStream = null);
}

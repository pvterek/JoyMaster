using Grpc.Core;
using Server.Models;
using Server.Protos;

namespace Server.Services.Interfaces;

internal interface IConnectionService
{
    Task RegisterAsync(string connectionGuid, int clientId, IServerStreamWriter<Response> responseStream);
    Task CloseAsync(string connectionGuid);

    Connection? Get(string connectionGuid);
}

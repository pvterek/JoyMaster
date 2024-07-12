using Grpc.Core;
using Server.Models;
using Server.Protos;

namespace Server.Services.Interfaces;

public interface IConnectionService
{
    Task RegisterAsync(string connectionGuid, int clientId, IServerStreamWriter<Response> responseStream);
    Task CloseAsync(string connectionGuid);
    KeyValuePair<Connection, IServerStreamWriter<Response>> GetActive(string connectionGuid);
    List<int> GetIdsList();
    bool ConnectionExists(string connectionGuid);
}

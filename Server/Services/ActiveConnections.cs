using Grpc.Core;
using Server.Models;
using Server.Protos;
using Server.Services.Interfaces;
using System.Collections.Concurrent;

namespace Server.Services;

public class ActiveConnections : IActiveConnections
{
    public ConcurrentDictionary<Connection, IServerStreamWriter<Response>> Connections { get; } = new();

    public KeyValuePair<Connection, IServerStreamWriter<Response>> GetActiveConnection(string connectionGuid)
    {
        return Connections.FirstOrDefault(pair => pair.Key.ConnectionGuid == connectionGuid);
    }
}

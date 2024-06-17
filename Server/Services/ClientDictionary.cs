using Grpc.Core;
using Server.Models;
using Server.Protos;
using Server.Services.Interfaces;
using System.Collections.Concurrent;

namespace Server.Services;

public class ClientDictionary : IClientDictionary
{
    public ConcurrentDictionary<ClientModel, IServerStreamWriter<CommandReply>> Clients { get; } = new();
}

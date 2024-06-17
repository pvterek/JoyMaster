using Grpc.Core;
using Server.Models;
using Server.Protos;
using System.Collections.Concurrent;

namespace Server.Services.Interfaces;

public interface IClientDictionary
{
    ConcurrentDictionary<ClientModel, IServerStreamWriter<CommandReply>> Clients { get; }
}

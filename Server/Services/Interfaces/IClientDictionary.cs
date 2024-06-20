using Grpc.Core;
using Server.Models;
using Server.Protos;
using System.Collections.Concurrent;

namespace Server.Services.Interfaces;

public interface IClientDictionary
{
    ConcurrentDictionary<Client, IServerStreamWriter<CommandReply>> Clients { get; }
}

using Grpc.Core;
using Server.Models;
using Server.Protos;
using System.Collections.Concurrent;

namespace Server.Services.Interfaces
{
    public interface IHandlerHelper
    {
        public ClientModel? GetClientByIp(ConcurrentDictionary<ClientModel, IServerStreamWriter<CommandReply>> clientsList, string clientIpAddress);

        public void RemoveClientByIp(ConcurrentDictionary<ClientModel, IServerStreamWriter<CommandReply>> clientsList, string clientIpAddress, string reason, LoggerService logger);
    }
}

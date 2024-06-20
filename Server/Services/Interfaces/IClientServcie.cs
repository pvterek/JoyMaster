using Grpc.Core;
using Server.Models;
using Server.Protos;

namespace Server.Services.Interfaces;

internal interface IClientService
{
    Client? GetClientByIp(string clientIpAddress);
    Task RemoveClientByIpAsync(string clientIpAddress, string reason);
    Task RegisterClientAsync(IServerStreamWriter<CommandReply> responseStream, string clientId, string clientName, string clientAddress);
}

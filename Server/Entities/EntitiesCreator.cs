using Server.Models;
using Server.Protos;

namespace Server.Entities;

public class EntitiesCreator
{
    public Connection CreateConnection(string connectionGuid, int clientId)
    {
        return new Connection()
        {
            ConnectionGuid = connectionGuid,
            ClientId = clientId
        };
    }

    public Response CreateResponse(string command, string parameters)
    {
        return new Response()
        {
            Command = command.ToLower(),
            Parameters = parameters
        };
    }

    public Client CreateClient(string name, string ipAddress)
    {
        return new Client
        {
            Name = name,
            IpAddress = ipAddress
        };
    }
}

using Server.Models;

namespace Server.Repository;

public interface IClientRepository
{
    Task AddAsync(Client client);
    Task<Client?> GetAsync(string clientIpAddress, string clientName);
    Task<IEnumerable<Client>> GetAllAsync();
    Task DeleteAsync(int id);
    Task<List<ClientConnectionViewModel>> GetClientConnectionsAsync(List<int> connectionIds);
}

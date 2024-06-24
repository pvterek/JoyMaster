using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Repository;

public class ClientRepository(
    ApplicationDbContext context,
    ILogger<ClientRepository> logger
    ) : IClientRepository
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<ClientRepository> _logger = logger;

    public async Task AddAsync(Client client)
    {
        await _context.AddAsync(client);
        await _context.SaveChangesAsync();
    }

    public async Task<Client?> GetAsync(string clientName, string clientIpAddress)
    {
        return await _context.Clients.FirstOrDefaultAsync(
            c => c.Name == clientName && c.IpAddress == clientIpAddress);
    }

    public async Task<IEnumerable<Client>> GetAllAsync()
    {
        return await _context.Clients.Include(a => a.Connection).ToListAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client is not null)
        {
            _context.Clients.Remove(client);
        }
    }

    public async Task<List<ClientConnectionViewModel>> GetClientConnectionsAsync(List<int> connectionIds)
    {
        return await _context.Connections
            .Where(c => connectionIds.Contains(c.Id))
            .Include(c => c.Client)
            .Select(c => new ClientConnectionViewModel
            {
                Client = c.Client,
                Connection = c
            })
            .ToListAsync();
    }
}

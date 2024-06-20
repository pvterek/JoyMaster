using Server.Data;
using Server.Models;

namespace Server.Repositories;

public class ClientRepository(ApplicationDbContext context)
{
    private readonly ApplicationDbContext _context = context;

    public async Task AddClientAsync(Client client)
    {
        await _context.AddAsync(client);
        await _context.SaveChangesAsync();
    }

    //public async Task<IEnumerable<Client>> GetAllClientsAsync()
    //{
    //    return await _context.Clients.Include(a => a.Connections).ToListAsync();
    //}

    //public async Task DeleteClientAsync(int Id)
    //{
    //    var client = _context.Clients.FindAsync(x => x.Id == Id);
    //    if (client != null) 
    //    {
    //        _context.Clients.Remove(client);
    //    }
    //}
}

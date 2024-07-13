using Server.Data;
using Server.Models;
using Server.Repository.Interfaces;

namespace Server.Repository;

public class ConnectionRepository(ApplicationDbContext context) : IConnectionRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task AddAsync(Connection connection)
    {
        await _context.AddAsync(connection);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Connection connection)
    {
        _context.Update(connection);
        await _context.SaveChangesAsync();
    }
}

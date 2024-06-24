using Server.Data;
using Server.Models;

namespace Server.Repository;

public class ConnectionRepository(
    ApplicationDbContext context,
    ILogger<ConnectionRepository> logger
    ) : IConnectionRepository
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<ConnectionRepository> _logger = logger;

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

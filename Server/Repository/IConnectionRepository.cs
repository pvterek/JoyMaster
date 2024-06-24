using Server.Models;

namespace Server.Repository;

public interface IConnectionRepository
{
    Task AddAsync(Connection connection);
    Task UpdateAsync(Connection connection);
}

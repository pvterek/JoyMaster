using Server.Models;

namespace Server.Repository.Interfaces;

public interface IConnectionRepository
{
    Task AddAsync(Connection connection);
    Task UpdateAsync(Connection connection);
}

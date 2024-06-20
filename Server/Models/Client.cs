namespace Server.Models;

public class Client
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required string IpAddress { get; set; }

    public required DateTime LastConnectionDate { get; set; }

    public ICollection<Connection> Connection { get; } = [];
}

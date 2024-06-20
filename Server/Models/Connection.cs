namespace Server.Models;

public class Connection
{
    public int Id { get; set; }

    public DateTime ConnectedTime { get; set; }

    public DateTime DisconnectedTime { get; set; }

    public string ClientId { get; set; } = null!;

    public Client Client { get; set; } = null!;
}

namespace Server.Models;

public class Connection
{
    public int Id { get; set; }
    public required string ConnectionGuid { get; set; }
    public DateTime ConnectedTime { get; } = DateTime.Now;
    public DateTime? DisconnectedTime { get; set; }
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
}

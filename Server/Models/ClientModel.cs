namespace Server.Models;

public class ClientModel
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required string AddressIp { get; set; }

    public required DateTime LastConnectionDate { get; set; }
}

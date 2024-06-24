namespace Server.Models;

public class ClientConnectionViewModel
{
    public required Client Client { get; set; }

    public required Connection Connection { get; set; }
}

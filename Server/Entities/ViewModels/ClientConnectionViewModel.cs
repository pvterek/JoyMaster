using Server.Models;

namespace Server.Entities.ViewModels;

public class ClientConnectionViewModel
{
    public required Client Client { get; set; }
    public required Connection Connection { get; set; }
}

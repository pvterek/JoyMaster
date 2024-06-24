namespace Server.Models;

public class Message
{
    public required string ConnectionGuid { get; set; }

    public string? MessageContent { get; set; }
}

namespace Server.Models;

public class Message
{
    public required string ClientId { get; set; }

    public string? MessageContent { get; set; }
}

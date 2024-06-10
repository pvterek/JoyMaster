namespace Server.Models;

public class MessageModel
{
    public required string ClientId { get; set; }

    public string? Message { get; set; }
}

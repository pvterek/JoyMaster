namespace Server.Models;

public class CommandModel
{
    public required string ClientId { get; set; }

    public string? Command { get; set; }
}

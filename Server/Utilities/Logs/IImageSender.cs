using Server.Entities;

namespace Server.Utilities.Logs;

public interface IImageSender
{
    Task SendImageAsync(ImageData imageData);
}

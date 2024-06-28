using Google.Protobuf;
using Server.Protos;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;

namespace Client.Services;

public class ScreenCaptureService(ConnectionService clientService)
{
    private readonly ConnectionService _clientService = clientService;

    public DesktopFrame CaptureDesktopFrame()
    {
        byte[] screenshotBytes = TakeScreenshot();

        var desktopFrame = new DesktopFrame
        {
            Id = _clientService.ConnectionGuid,
            Image = ByteString.CopyFrom(screenshotBytes)
        };

        return desktopFrame;
    }

    private byte[] TakeScreenshot()
    {
        var screenWidth = (int)SystemParameters.PrimaryScreenWidth;
        var screenHeight = (int)SystemParameters.PrimaryScreenHeight;

        using var bmp = new Bitmap(screenWidth, screenHeight);
        using (var g = Graphics.FromImage(bmp))
        {
            g.CopyFromScreen(0, 0, 0, 0, bmp.Size);
        }

        using var memoryStream = new MemoryStream();
        bmp.Save(memoryStream, ImageFormat.Png);

        return memoryStream.ToArray();
    }
}

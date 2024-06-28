using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;

namespace Client.Services;

public class ScreenCapture
{
    public static byte[] TakeScreenshot()
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

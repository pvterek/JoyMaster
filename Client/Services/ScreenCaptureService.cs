using Google.Protobuf;
using Server.Protos;

namespace Client.Services;

class ScreenCaptureService
{
    public static DesktopFrame CaptureDesktopFrame()
    {
        byte[] screenshotBytes = ScreenCapture.TakeScreenshot();

        var desktopFrame = new DesktopFrame
        {
            Id = ClientService.ConnectionGuid,
            Image = ByteString.CopyFrom(screenshotBytes)
        };

        return desktopFrame;
    }
}

using Google.Protobuf;
using Server.Entities;

namespace Server.Services;

public class ImageDataHelper
{
    public ImageData CreateImageDataEntity(string connectionGuid, ByteString imageData)
    {
        return new ImageData()
        {
            ConnectionGuid = connectionGuid,
            Base64Image = ConvertImageToBase64(imageData)
        };
    }

    private string ConvertImageToBase64(ByteString imageData)
    {
        return Convert.ToBase64String(imageData.ToByteArray());
    }
}

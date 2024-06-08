using Server.Models;
using static Server.Services.HandlerService; //get rid of it

namespace Server.Services;

public class HandlerHelper
{
    //public static byte[] GetLatestFrame()
    //{
    //    byte[] helperFrame = null;
    //
    //    if (Frames.IsEmpty)
    //    {
    //        return null;
    //    }
    //    else if (Frames.TryDequeue(out var frame))
    //    {
    //        helperFrame = frame;
    //        return frame;
    //    }
    //
    //    return helperFrame;
    //}

    public static ClientModel? GetClientByIp(string addressIp)
    {
        return ConnectedClients.Keys.SingleOrDefault(client => client.AddressIp == addressIp);
    }
}

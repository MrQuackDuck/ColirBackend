namespace Colir.BLL.RequestModels.Room;

public class RequestToClearRoom
{
    public long IssuerId { get; set; }
    public string RoomGuid { get; set; } = default!;
}
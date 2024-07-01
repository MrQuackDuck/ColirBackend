namespace Colir.BLL.RequestModels.Room;

public class RequestToGetRoomInfo
{
    public long IssuerId { get; set; }
    public string RoomGuid { get; set; } = default!;
}
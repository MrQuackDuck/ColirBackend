namespace Colir.BLL.RequestModels.Room;

public class RequestToJoinRoom
{
    public long IssuerId { get; set; }
    public string RoomGuid { get; set; } = default!;
}
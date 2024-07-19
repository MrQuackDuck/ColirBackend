namespace Colir.BLL.RequestModels.Room;

public class RequestToLeaveFromRoom
{
    public long IssuerId { get; set; }
    public string RoomGuid { get; set; } = default!;
}
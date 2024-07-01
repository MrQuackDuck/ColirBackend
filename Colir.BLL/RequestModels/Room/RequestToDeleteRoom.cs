namespace Colir.BLL.RequestModels.Room;

public class RequestToDeleteRoom
{
    public long IssuerId { get; set; }
    public string RoomGuid { get; set; } = default!;
}
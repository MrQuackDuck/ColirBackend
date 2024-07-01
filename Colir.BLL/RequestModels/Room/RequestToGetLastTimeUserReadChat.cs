namespace Colir.BLL.RequestModels.Room;

public class RequestToGetLastTimeUserReadChat
{
    public long IssuerId { get; set; }
    public string RoomGuid { get; set; } = default!;
}
namespace Colir.BLL.RequestModels.Room;

public class RequestToUpdateLastTimeUserReadChat
{
    public long IssuerId { get; set; }
    public string RoomGuid { get; set; } = default!;
}
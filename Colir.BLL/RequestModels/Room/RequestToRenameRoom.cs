namespace Colir.BLL.RequestModels.Room;

public class RequestToRenameRoom
{
    public long IssuerId { get; set; }
    public string RoomGuid { get; set; } = default!;
    public string NewName { get; set; } = default!;
}
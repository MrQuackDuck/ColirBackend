namespace Colir.BLL.RequestModels.Room;

public class RequestToKickMember
{
    public long IssuerId { get; set; }
    public int TargetHexId { get; set; }
    public string RoomGuid { get; set; } = default!;
}
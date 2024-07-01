namespace Colir.BLL.RequestModels.Room;

public class RequestToKickMember
{
    public long IssuerId { get; set; }
    public string TargetHexId { get; set; } = default!;
    public string RoomGuid { get; set; } = default!;
}
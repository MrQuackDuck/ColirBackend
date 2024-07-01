namespace Colir.HttpModels.Room;

public class KickMemberModel
{
    public string TargetHexId { get; set; } = default!;
    public string RoomGuid { get; set; } = default!;
}
namespace Colir.HttpModels.Room;

public class KickMemberModel
{
    public int TargetHexId { get; set; } = default!;
    public string RoomGuid { get; set; } = default!;
}
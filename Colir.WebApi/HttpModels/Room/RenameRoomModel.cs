namespace Colir.HttpModels.Room;

public class RenameRoomModel
{
    public string RoomGuid { get; set; } = default!;
    public string NewName { get; set; } = default!;
}
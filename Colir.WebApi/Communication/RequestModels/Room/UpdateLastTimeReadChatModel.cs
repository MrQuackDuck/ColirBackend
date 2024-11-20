namespace Colir.Communication.RequestModels.Room;

public class UpdateLastTimeReadChatModel
{
    public string RoomGuid { get; set; } = default!;
    public DateTime LastTimeRead { get; set; } = default!;
}
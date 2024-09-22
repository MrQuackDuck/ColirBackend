namespace Colir.Communication.Models;

public class ChatUser
{
    public long HexId { get; set; }
    public string ConnectionId { get; set; } = default!;
    public string RoomGuid { get; set; } = default!;
}
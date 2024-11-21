namespace Colir.Communication.RequestModels.Room;

public class UpdateLastReadMessageModel
{
    public string RoomGuid { get; set; } = default!;
    public long? MessageId { get; set; }
}
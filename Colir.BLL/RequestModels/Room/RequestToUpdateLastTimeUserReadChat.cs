namespace Colir.BLL.RequestModels.Room;

public class RequestToUpdateLastReadMessageByUser
{
    public long IssuerId { get; set; }
    public string RoomGuid { get; set; } = default!;
    public long? MessageId { get; set; }
}
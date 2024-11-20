namespace Colir.BLL.RequestModels.Room;

public class RequestToGetUnreadReplies
{
    public long IssuerId { get; set; }
    public string RoomGuid { get; set; } = default!;
}
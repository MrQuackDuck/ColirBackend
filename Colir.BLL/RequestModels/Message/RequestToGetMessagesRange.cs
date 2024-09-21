namespace Colir.BLL.RequestModels.Message;

public class RequestToGetMessagesRange
{
    public long IssuerId { get; set; }
    public string RoomGuid { get; set; } = default!;
    public long StartId { get; set; }
    public long EndId { get; set; }
}
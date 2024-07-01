namespace Colir.BLL.RequestModels.Message;

public class RequestToGetLastMessages
{
    public long IssuerId { get; set; }
    public int Count { get; set; }
    public int SkipCount { get; set; }
    public string RoomGuid { get; set; } = default!;
}
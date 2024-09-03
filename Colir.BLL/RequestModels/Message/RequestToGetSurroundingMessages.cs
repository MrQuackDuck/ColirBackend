namespace Colir.BLL.RequestModels.Message;

public class RequestToGetSurroundingMessages
{
    public long IssuerId { get; set; }
    public long MessageId { get; set; }
    public int Count { get; set; }
}
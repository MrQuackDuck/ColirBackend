namespace Colir.BLL.RequestModels.Message;

public class RequestToAddReactionOnMessage
{
    public long IssuerId { get; set; }
    public long MessageId { get; set; }
    public string Reaction { get; set; } = default!;
}
namespace Colir.BLL.RequestModels.Message;

public class RequestToRemoveReactionFromMessage
{
    public long IssuerId { get; set; }
    public long ReactionId { get; set; }
}
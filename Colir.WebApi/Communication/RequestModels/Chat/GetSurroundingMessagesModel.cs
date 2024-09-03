namespace Colir.Communication.RequestModels.Chat;

public class GetSurroundingMessagesModel
{
    public long MessageId { get; set; }
    public int Count { get; set; }
}
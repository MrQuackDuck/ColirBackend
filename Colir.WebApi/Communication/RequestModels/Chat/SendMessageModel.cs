namespace Colir.Communication.RequestModels.Chat;

public class SendMessageModel
{
    public string Content { get; set; } = default!;
    public List<long> AttachmentsIds { get; set; } = default!;
    public long? ReplyMessageId { get; set; }
}
namespace Colir.HttpModels.Message;

public class SendMessageModel
{
    public string Content { get; set; } = default!;
    public List<int> AttachmentsIds { get; set; } = default!;
    public string RoomGuid { get; set; } = default!;
    public long? ReplyMessageId { get; set; }
}
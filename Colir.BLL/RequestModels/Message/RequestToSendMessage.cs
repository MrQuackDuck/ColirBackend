namespace Colir.BLL.RequestModels.Message;

public class RequestToSendMessage
{
    public long IssuerId { get; set; }
    public string Content { get; set; } = default!;
    public List<long>? AttachmentsIds { get; set; } = default!;
    public string RoomGuid { get; set; } = default!;
    public long? ReplyMessageId { get; set; }
}
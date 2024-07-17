namespace Colir.HttpModels.Message;

public class SendMessageModel
{
    public string Content { get; set; } = default!;
    public List<IFormFile> Attachments { get; set; } = default!;
    public string RoomGuid { get; set; } = default!;
    public long? ReplyMessageId { get; set; }
}
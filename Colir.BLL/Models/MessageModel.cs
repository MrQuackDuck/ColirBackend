namespace Colir.BLL.Models;

public class MessageModel
{
    public long Id { get; set; }
    public string RoomGuid { get; set; } = default!;
    public long AuthorHexId { get; set; }
    public DateTime PostDate { get; set; }
    public DateTime? EditDate { get; set; }
    public string Content { get; set; } = default!;
    public long? RepliedMessageId { get; set; }
    public MessageModel? RepliedMessage { get; set; }

    public List<ReactionModel> Reactions { get; set; } = default!;
    public List<AttachmentModel> Attachments { get; set; } = default!;
}
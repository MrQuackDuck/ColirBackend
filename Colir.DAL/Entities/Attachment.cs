using System.ComponentModel.DataAnnotations.Schema;
using DAL.Enums;

namespace DAL.Entities;

public class Attachment : BaseEntity
{
    public string Filename { get; set; } = default!;
    public string Path { get; set; } = default!;
    public AttachmentType AttachmentType;
    public long SizeInKb { get; set; }

    [ForeignKey(nameof(Message))]
    public long MessageId { get; set; }

    public Message Message { get; set; } = default!;
}
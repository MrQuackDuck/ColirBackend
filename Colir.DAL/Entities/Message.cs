#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

public class Message : BaseEntity
{
    [MaxLength(4096)]
    public string Content { get; set; } = default!;
    public DateTime PostDate { get; set; }
    public DateTime? EditDate { get; set; }

    [ForeignKey(nameof(Room))]
    public long RoomId;

    [ForeignKey(nameof(Author))]
    public long AuthorId;

    [ForeignKey(nameof(RepliedTo))]
    public long RepliedMessageId;

    public Room Room { get; set; } = default!;
    public User Author { get; set; } = default!;
    public Message? RepliedTo;
    public List<Attachment> Attachments { get; set; } = default!;
    public List<Reaction> Reactions { get; set; } = default!;
}
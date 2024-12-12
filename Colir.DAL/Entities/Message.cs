#nullable enable
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Entities;

public class Message : BaseEntity
{
    public string Content { get; set; } = default!;
    public DateTime PostDate { get; set; }
    public DateTime? EditDate { get; set; }

    [ForeignKey(nameof(Room))]
    public long RoomId { get; set; }

    [ForeignKey(nameof(Author))]
    public long? AuthorId { get; set; }

    [ForeignKey(nameof(RepliedTo))]
    public long? RepliedMessageId { get; set; }

    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Room Room { get; set; } = default!;

    [DeleteBehavior(DeleteBehavior.SetNull)]
    public User? Author { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public Message? RepliedTo { get; set; }

    public List<Attachment> Attachments { get; set; } = default!;
    public List<Reaction> Reactions { get; set; } = default!;
}
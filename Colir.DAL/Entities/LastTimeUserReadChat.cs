using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Entities;

public class LastTimeUserReadChat : BaseEntity
{
    public DateTime Timestamp { get; set; }

    [ForeignKey(nameof(Room))]
    public long? RoomId { get; set; }

    [ForeignKey(nameof(User))]
    public long? UserId { get; set; }

    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Room? Room { get; set; }

    [DeleteBehavior(DeleteBehavior.Cascade)]
    public User? User { get; set; }
}
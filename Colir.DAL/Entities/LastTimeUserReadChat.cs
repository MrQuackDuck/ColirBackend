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
    
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Room Room { get; set; } = default!;
    
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public User User { get; set; } = default!;
}
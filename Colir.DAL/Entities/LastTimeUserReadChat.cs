using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities;

public class LastTimeUserReadChat : BaseEntity
{
    public DateTime Timestamp { get; set; }

    [ForeignKey(nameof(Room))]
    public long RoomId { get; set; }
    
    [ForeignKey(nameof(User))]
    public long UserId { get; set; }
    
    public Room Room { get; set; } = default!;
    public User User { get; set; } = default!;
}
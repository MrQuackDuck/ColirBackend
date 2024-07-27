using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Entities;

public class UserToRoom
{
    [ForeignKey(nameof(User))]
    public long UserId { get; set; }
    
    [ForeignKey(nameof(Room))]
    public long RoomId { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public User User { get; set; } = default!;
    
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Room Room { get; set; } = default!;
}
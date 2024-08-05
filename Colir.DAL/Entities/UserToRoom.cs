using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Entities;

public class UserToRoom
{
    [ForeignKey(nameof(User))]
    public long UserId { get; set; }

    [ForeignKey(nameof(Room))]
    public long RoomId { get; set; }

    [DeleteBehavior(DeleteBehavior.ClientNoAction)]
    public User User { get; set; } = default!;

    [DeleteBehavior(DeleteBehavior.ClientNoAction)]
    public Room Room { get; set; } = default!;
}
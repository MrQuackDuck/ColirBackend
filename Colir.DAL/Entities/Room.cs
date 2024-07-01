using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.Entities;

[Index(nameof(Guid), IsUnique = true)]
public class Room : BaseEntity
{
    public string Guid { get; set; } = default!;
    public string Name { get; set; } = default!;
    public DateTime? ExpiryDate = default;

    [ForeignKey(nameof(Owner))]
    public long OwnerId = default!;

    public User Owner { get; set; } = default!;
    public ICollection<User> JoinedUsers { get; set; } = default!;
}
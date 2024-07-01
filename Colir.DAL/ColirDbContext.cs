using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class ColirDbContext : DbContext
{
    public ColirDbContext(DbContextOptions<ColirDbContext> options) : base(options) { }
    
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<LastTimeUserReadChat> LastTimeUserReadChats { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }
    public DbSet<UserStatistics> UserStatistics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>()
            .HasOne<User>(nameof(Room.Owner));

        modelBuilder.Entity<Room>()
            .HasMany(r => r.JoinedUsers)
            .WithMany(u => u.JoinedRooms)
            .UsingEntity<UserToRoom>();
    }
}
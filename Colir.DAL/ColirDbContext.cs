using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(warnings =>
        {
            warnings.Ignore(CoreEventId.ForeignKeyAttributesOnBothNavigationsWarning);
            warnings.Ignore(CoreEventId.ForeignKeyAttributesOnBothPropertiesWarning);
        });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Room>()
            .HasOne<User>(nameof(Room.Owner));

        modelBuilder.Entity<Room>()
            .HasMany(r => r.JoinedUsers)
            .WithMany(u => u.JoinedRooms)
            .UsingEntity<UserToRoom>();
        
        modelBuilder.Entity<User>()
            .HasOne(s => s.UserSettings)
            .WithOne(s => s.User)
            .HasForeignKey<UserSettings>(us => us.UserId)
            .IsRequired();
        
        modelBuilder.Entity<User>()
            .HasOne(s => s.UserStatistics)
            .WithOne(s => s.User)
            .HasForeignKey<UserStatistics>(us => us.UserId)
            .IsRequired();

        modelBuilder.Entity<Message>()
            .HasMany(s => s.Attachments)
            .WithOne(s => s.Message)
            .HasForeignKey(a => a.MessageId);
        
        modelBuilder.Entity<Message>()
            .HasMany(s => s.Reactions)
            .WithOne(s => s.Message)
            .HasForeignKey(r => r.MessageId)
            .IsRequired();
    }
}
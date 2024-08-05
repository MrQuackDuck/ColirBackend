using DAL;
using DAL.Entities;
using DAL.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Colir.DAL.Tests.Utils;

public static class UnitTestHelper
{
    public static ColirDbContext CreateDbContext()
    {
        // Create database options (in-memory for unit testing)
        var options = new DbContextOptionsBuilder<ColirDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Mock<IConfiguration> configMock = new Mock<IConfiguration>();
        configMock.Setup(config => config["DatabaseEncryption:EncryptionPassword"]).Returns("16-char-password");
        configMock.Setup(config => config["DatabaseEncryption:InitializationVector"]).Returns("16-char-invector");

        return new ColirDbContext(options, configMock.Object);
    }

    public static void SeedData(ColirDbContext context)
    {
        // Users
        var user1 = new User
        {
            Id = 1,
            HexId = 0xFFFFFF,
            Username = "First User",
            GitHubId = "2024",
            AuthType = UserAuthType.Anonymous
        };

        var user2 = new User
        {
            Id = 2,
            HexId = 0x000000,
            Username = "Second User",
            AuthType = UserAuthType.Anonymous
        };

        var user3 = new User
        {
            Id = 3,
            HexId = 0xF4CA16,
            Username = "Third User",
            AuthType = UserAuthType.Anonymous,
        };

        context.Users.AddRange(user1, user2, user3);

        // Rooms
        var defaultRoom = new Room
        {
            Id = 1,
            Guid = "cbaa8673-ea8b-43f8-b4cc-b8b0797b620e",
            Name = "Room #1",
            ExpiryDate = DateTime.Now.Add(new TimeSpan(1, 0, 0)),
            OwnerId = 1, // "First User"
            JoinedUsers = new List<User>() { user1, user2 },
        };

        var expiredRoom = new Room
        {
            Id = 2,
            Guid = "12ffb712-aca7-416f-b899-8f9aaac6770f",
            Name = "Room #2",
            ExpiryDate = new DateTime(2000, 1, 1),
            OwnerId = 1, // "First User"
            JoinedUsers = new List<User>() { user1, user2 },
        };

        context.Rooms.AddRange(defaultRoom, expiredRoom);

        // Attachments
        var attachment1 = new Attachment
        {
            Id = 1,
            Filename = "file.zip",
            Path = "/tests/file.zip",
            SizeInBytes = 4,
        };

        context.Attachments.Add(attachment1);

        // Messages
        var message1 = new Message
        {
            Id = 1,
            Content = "Message in Room #1",
            PostDate = DateTime.Now,
            RoomId = defaultRoom.Id, // "Room #1"
            AuthorId = user1.Id, // "First User"
            Attachments = new List<Attachment>() { attachment1 },
        };

        var message2 = new Message
        {
            Id = 2,
            Content = "Reply to first message",
            PostDate = DateTime.Now,
            RoomId = defaultRoom.Id, // "Room #1"
            AuthorId = user1.Id, // "First User"
            RepliedMessageId = message1.Id, // "Message in Room #1"
        };

        var message3 = new Message
        {
            Id = 3,
            Content = "Message in expired room",
            PostDate = DateTime.Now,
            RoomId = expiredRoom.Id, // "Room #2"
            AuthorId = user1.Id, // "First User"
            RepliedMessageId = message1.Id, // "Message in Room #1"
        };

        context.Messages.AddRange(message1, message2, message3);

        // Reactions
        var reaction1 = new Reaction
        {
            Id = 1,
            Symbol = "🤣",
            AuthorId = user1.Id, // "First User"
            MessageId = message1.Id, // "Message in Room #1"
        };

        context.Reactions.Add(reaction1);

        // Last time users read chats
        var lastTimeFirstUserReadChat = new LastTimeUserReadChat
        {
            Id = 1,
            Room = defaultRoom, // "Room #1"
            User = user1, // "First User"
            Timestamp = DateTime.Now
        };

        context.LastTimeUserReadChats.Add(lastTimeFirstUserReadChat);

        // Save changes
        context.SaveChanges();
    }
}
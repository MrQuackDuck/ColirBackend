using DAL;
using DAL.Entities;
using DAL.Enums;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Colir.BLL.Tests.Utils;

public static class UnitTestHelper
{
    public static ColirDbContext CreateDbContext()
    {
        // Create database options (in-memory for unit testing)
        var options = new DbContextOptionsBuilder<ColirDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(config =>
            {
                config.Ignore(InMemoryEventId.TransactionIgnoredWarning);
                config.Ignore(CoreEventId.ForeignKeyAttributesOnBothNavigationsWarning);
                config.Ignore(CoreEventId.ForeignKeyAttributesOnBothPropertiesWarning);
            })
            .Options;

        Mock<IConfiguration> configMock = new Mock<IConfiguration>();
        configMock.Setup(config => config["DatabaseEncryption:EncryptionPassword"]).Returns("16-char-password");
        configMock.Setup(config => config["DatabaseEncryption:InitializationVector"]).Returns("16-char-invector");

        Mock<IRoomFileManager> roomFileManagerMock = new Mock<IRoomFileManager>();

        return new ColirDbContext(options, configMock.Object, roomFileManagerMock.Object);
    }

    public static void SeedData(ColirDbContext context)
    {
        // Users
        var user1 = new User
        {
            Id = 1,
            HexId = 0xFFFFFF,
            Username = "First User",
            AuthType = UserAuthType.Anonymous,
            UserSettings = new UserSettings()
            {
                StatisticsEnabled = true
            }
        };

        var user2 = new User
        {
            Id = 2,
            HexId = 0x000000,
            Username = "Second User",
            AuthType = UserAuthType.Anonymous,
        };

        var user3 = new User
        {
            Id = 3,
            HexId = 0xF4CA16,
            Username = "Third User",
            AuthType = UserAuthType.Anonymous,
            UserSettings = new UserSettings()
            {
                StatisticsEnabled = true
            }
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
            Path = "/cbaa8673-ea8b-43f8-b4cc-b8b0797b620e/tests/file.zip",
            SizeInBytes = 4,
        };

        var attachment2 = new Attachment
        {
            Id = 2,
            Filename = "file.zip",
            Path = "/12ffb712-aca7-416f-b899-8f9aaac6770f/tests/file.zip",
            SizeInBytes = 4,
        };

        context.Attachments.AddRange(attachment1, attachment2);

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
            AuthorId = user3.Id, // "Third User"
            RepliedMessageId = message1.Id, // "Message in Room #1"
        };

        var message3 = new Message
        {
            Id = 3,
            Content = "Another message in Room #1",
            PostDate = DateTime.Now,
            RoomId = defaultRoom.Id, // "Room #1"
            AuthorId = user1.Id, // "First User"
        };

        var message4 = new Message
        {
            Id = 4,
            Content = "Another message in Room #1",
            PostDate = DateTime.Now,
            RoomId = defaultRoom.Id, // "Room #1"
            AuthorId = user3.Id, // "Third   User"
        };

        var message5 = new Message
        {
            Id = 5,
            Content = "Another message in Room #2",
            PostDate = DateTime.Now,
            RoomId = expiredRoom.Id, // "Room #2 (expired)"
            AuthorId = user1.Id, // "First User"
        };

        context.Messages.AddRange(message1, message2, message3, message4, message5);

        // Reactions
        var reaction1 = new Reaction
        {
            Id = 1,
            Symbol = "🤣",
            AuthorId = user1.Id, // "First User"
            MessageId = message1.Id, // "Message in Room #1"
        };

        var reaction2 = new Reaction
        {
            Id = 2,
            Symbol = "🙄",
            AuthorId = user3.Id, // "Third User"
            MessageId = message1.Id, // "Message in Room #1"
        };

        var reaction3 = new Reaction
        {
            Id = 3,
            Symbol = "🙄",
            AuthorId = user3.Id, // "Third User"
            MessageId = message5.Id, // Another message in Room #1
        };

        context.Reactions.AddRange(reaction1, reaction2, reaction3);

        // Last time users read chats
        var lastTimeFirstUserReadChat = new LastTimeUserReadChat
        {
            Id = 1,
            Room = defaultRoom, // "Room #1"
            User = user1, // "First User"
            Timestamp = DateTime.Now - new TimeSpan(1, 0, 0),
        };

        var lastTimeSecondUserReadChat = new LastTimeUserReadChat
        {
            Id = 2,
            Room = defaultRoom, // "Room #1"
            User = user2, // "Second User"
            Timestamp = DateTime.Now
        };

        context.LastTimeUserReadChats.Add(lastTimeFirstUserReadChat);
        context.LastTimeUserReadChats.Add(lastTimeSecondUserReadChat);

        // Save changes
        context.SaveChanges();
    }
}
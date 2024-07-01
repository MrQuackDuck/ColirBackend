using DAL;
using DAL.Entities;
using DAL.Enums;

namespace Colir.DAL.Tests.Utils;

public static class UnitTestHelper
{
    public static void SeedData(ColirDbContext context)
    {
        // Users
        var user1 = new User
        {
            Id = 1,
            HexId = "#FFFFFF",
            Username = "First User",
            AuthType = UserAuthType.Anonymous,
        };

        var user2 = new User
        {
            Id = 2,
            HexId = "#000000",
            Username = "Second User",
            AuthType = UserAuthType.Anonymous,
        };

        context.Users.AddRange(user1, user2);

        // Rooms
        var defaultRoom = new Room
        {
            Id = 1,
            Guid = Guid.NewGuid().ToString(),
            Name = "Room #1",
            ExpiryDate = DateTime.Now.Add(new TimeSpan(1, 0, 0)),
            OwnerId = 1, // "First User"
            JoinedUsers = new List<User>() { user1, user2 },
        };

        var expiredRoom = new Room
        {
            Id = 2,
            Guid = Guid.NewGuid().ToString(),
            Name = "Room #2",
            ExpiryDate = new DateTime(2000, 1, 1),
            OwnerId = 1, // "First User",
            JoinedUsers = new List<User>() { user1, user2 },
        };
        
        context.Rooms.AddRange(defaultRoom, expiredRoom);

        // Messages
        var message1 = new Message
        {
            Id = 1,
            Content = "Message in Room #1",
            PostDate = DateTime.Now,
            RoomId = defaultRoom.Id, // "Room #1"
            AuthorId = user1.Id, // "First User"
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
        
        context.Messages.AddRange(message1, message2);

        // Reactions
        var reaction1 = new Reaction
        {
            Id = 1,
            Symbol = ":)",
            AuthorId = user1.Id, // "First User"
            MessageId = message1.Id, // "Message in Room #1"
        };
        
        context.Reactions.Add(reaction1);

        // Attachments
        var attachment1 = new Attachment
        {
            Id = 1,
            Filename = "file.zip",
            Path = "/tests/file.zip",
            AttachmentType = AttachmentType.File,
            SizeInKb = 4,
        };

        context.Attachments.Add(attachment1);

        // Last time users read chats
        var lastTimeFirstUserReadChat = new LastTimeUserReadChat
        {
            Id = 1,
            RoomId = 1, // "Room #1"
            UserId = 1, // "First User"
        };
        
        context.LastTimeUserReadChats.Add(lastTimeFirstUserReadChat);

        // User statistics
        var firstUserStatistics = new UserStatistics
        {
            Id = 1,
            UserId = 1, // "First User"
            SecondsSpentInVoice = 0,
            ReactionsSet = 1,
            MessagesSent = 2,
            RoomsJoined = 2,
            RoomsCreated = 2,
        };

        var secondUserStatistics = new UserStatistics
        {
            Id = 2,
            UserId = 2, // "First User"
            SecondsSpentInVoice = 0,
            ReactionsSet = 0,
            MessagesSent = 0,
            RoomsJoined = 2,
            RoomsCreated = 0,
        };
        
        context.UserStatistics.AddRange(firstUserStatistics, secondUserStatistics);

        // User settings
        var firstUserSettings = new UserSettings
        {
            Id = 1,
            UserId = 1, // "First User"
            StatisticsEnabled = true,
        };

        var secondUserSettings = new UserSettings
        {
            Id = 2,
            UserId = 1, // "Second User"
            StatisticsEnabled = false,
        };
        
        context.UserSettings.AddRange(firstUserSettings, secondUserSettings);

        // Save changes
        context.SaveChanges();
    }
}
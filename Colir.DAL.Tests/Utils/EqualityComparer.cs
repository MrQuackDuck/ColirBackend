using DAL.Entities;

namespace Colir.DAL.Tests.Utils;

public class AttachmentEqualityComparer : IEqualityComparer<Attachment>
{
    public bool Equals(Attachment? x, Attachment? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.AttachmentType == y.AttachmentType && x.Filename == y.Filename && x.Path == y.Path && x.SizeInKb == y.SizeInKb && x.MessageId == y.MessageId && Equals(x.Message, y.Message);
    }

    public int GetHashCode(Attachment obj)
    {
        return HashCode.Combine((int)obj.AttachmentType, obj.Filename, obj.Path, obj.SizeInKb, obj.MessageId, obj.Message);
    }
}

public class LastTimeUserReadChatEqualityComparer : IEqualityComparer<LastTimeUserReadChat>
{
    public bool Equals(LastTimeUserReadChat? x, LastTimeUserReadChat? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.RoomId == y.RoomId && x.UserId == y.UserId && x.Timestamp.Equals(y.Timestamp);
    }

    public int GetHashCode(LastTimeUserReadChat obj)
    {
        return HashCode.Combine(obj.RoomId, obj.UserId, obj.Timestamp);
    }
}

public class MessageEqualityComparer : IEqualityComparer<Message>
{
    public bool Equals(Message? x, Message? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.RoomId == y.RoomId && x.AuthorId == y.AuthorId && x.RepliedMessageId == y.RepliedMessageId && Equals(x.RepliedTo, y.RepliedTo) && x.Content == y.Content && x.PostDate.Equals(y.PostDate) && Nullable.Equals(x.EditDate, y.EditDate) && x.Room.Equals(y.Room) && x.Author.Equals(y.Author) && x.Attachments.Equals(y.Attachments) && x.Reactions.Equals(y.Reactions);
    }

    public int GetHashCode(Message obj)
    {
        var hashCode = new HashCode();
        hashCode.Add(obj.RoomId);
        hashCode.Add(obj.AuthorId);
        hashCode.Add(obj.RepliedMessageId);
        hashCode.Add(obj.RepliedTo);
        hashCode.Add(obj.Content);
        hashCode.Add(obj.PostDate);
        hashCode.Add(obj.EditDate);
        hashCode.Add(obj.Room);
        hashCode.Add(obj.Author);
        hashCode.Add(obj.Attachments);
        hashCode.Add(obj.Reactions);
        return hashCode.ToHashCode();
    }
}

public class ReactionEqualityComparer : IEqualityComparer<Reaction>
{
    public bool Equals(Reaction? x, Reaction? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Symbol == y.Symbol && x.AuthorId == y.AuthorId && x.MessageId == y.MessageId && Equals(x.Author, y.Author) && Equals(x.Message, y.Message);
    }

    public int GetHashCode(Reaction obj)
    {
        return HashCode.Combine(obj.Symbol, obj.AuthorId, obj.MessageId, obj.Author, obj.Message);
    }
}

public class RoomEqualityComparer : IEqualityComparer<Room>
{
    public bool Equals(Room? x, Room? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return Nullable.Equals(x.ExpiryDate, y.ExpiryDate) && x.OwnerId == y.OwnerId && x.Guid == y.Guid && x.Name == y.Name && Equals(x.Owner, y.Owner) && Equals(x.JoinedUsers, y.JoinedUsers);
    }

    public int GetHashCode(Room obj)
    {
        return HashCode.Combine(obj.ExpiryDate, obj.OwnerId, obj.Guid, obj.Name, obj.Owner, obj.JoinedUsers);
    }
}

public class UserEqualityComparer : IEqualityComparer<User>
{
    public bool Equals(User? x, User? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.HexId == y.HexId && x.GitHubId == y.GitHubId && x.Username == y.Username && x.AuthType == y.AuthType && x.UserSettingsId == y.UserSettingsId && Equals(x.UserSettings, y.UserSettings) && Equals(x.JoinedRooms, y.JoinedRooms);
    }

    public int GetHashCode(User obj)
    {
        return HashCode.Combine(obj.HexId, obj.GitHubId, obj.Username, (int)obj.AuthType, obj.UserSettingsId, obj.UserSettings, obj.JoinedRooms);
    }
}

public class UserSettingsEqualityComparer : IEqualityComparer<UserSettings>
{
    public bool Equals(UserSettings? x, UserSettings? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.UserId == y.UserId && x.StatisticsEnabled == y.StatisticsEnabled && Equals(x.User, y.User);
    }

    public int GetHashCode(UserSettings obj)
    {
        return HashCode.Combine(obj.UserId, obj.StatisticsEnabled, obj.User);
    }
}

public class UserStatisticsEqualityComparer : IEqualityComparer<UserStatistics>
{
    public bool Equals(UserStatistics? x, UserStatistics? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.UserId == y.UserId && x.SecondsSpentInVoice == y.SecondsSpentInVoice && x.ReactionsSet == y.ReactionsSet && x.MessagesSent == y.MessagesSent && x.RoomsJoined == y.RoomsJoined && x.RoomsCreated == y.RoomsCreated && Equals(x.User, y.User);
    }

    public int GetHashCode(UserStatistics obj)
    {
        return HashCode.Combine(obj.UserId, obj.SecondsSpentInVoice, obj.ReactionsSet, obj.MessagesSent, obj.RoomsJoined, obj.RoomsCreated, obj.User);
    }
}
using System.Diagnostics.CodeAnalysis;
using DAL.Entities;

namespace Colir.DAL.Tests.Utils;

public class AttachmentEqualityComparer : IEqualityComparer<Attachment>
{
    public bool Equals([AllowNull] Attachment x, [AllowNull] Attachment y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Filename == y.Filename && x.Path == y.Path && x.SizeInKb == y.SizeInKb && x.MessageId == y.MessageId;
    }

    public int GetHashCode(Attachment obj)
    {
        return HashCode.Combine(obj.Filename, obj.Path, obj.SizeInKb, obj.MessageId);
    }
}

public class LastTimeUserReadChatEqualityComparer : IEqualityComparer<LastTimeUserReadChat>
{
    public bool Equals([AllowNull] LastTimeUserReadChat x, [AllowNull] LastTimeUserReadChat y)
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
    public bool Equals([AllowNull] Message x, [AllowNull] Message y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.RoomId == y.RoomId && x.AuthorId == y.AuthorId && x.RepliedMessageId == y.RepliedMessageId && x.Content == y.Content && x.PostDate.Equals(y.PostDate) && Nullable.Equals(x.EditDate, y.EditDate);
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
        return hashCode.ToHashCode();
    }
}

public class ReactionEqualityComparer : IEqualityComparer<Reaction>
{
    public bool Equals([AllowNull] Reaction x, [AllowNull] Reaction y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Symbol == y.Symbol && x.AuthorId == y.AuthorId && x.MessageId == y.MessageId;
    }

    public int GetHashCode(Reaction obj)
    {
        return HashCode.Combine(obj.Symbol, obj.AuthorId, obj.MessageId);
    }
}

public class RoomEqualityComparer : IEqualityComparer<Room>
{
    public bool Equals([AllowNull] Room x, [AllowNull] Room y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return Nullable.Equals(x.ExpiryDate, y.ExpiryDate) && x.OwnerId == y.OwnerId && x.Guid == y.Guid && x.Name == y.Name;
    }

    public int GetHashCode(Room obj)
    {
        return HashCode.Combine(obj.ExpiryDate, obj.OwnerId, obj.Guid, obj.Name);
    }
}

public class UserEqualityComparer : IEqualityComparer<User>
{
    public bool Equals([AllowNull] User x, [AllowNull] User y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.HexId == y.HexId && x.GitHubId == y.GitHubId && x.Username == y.Username && x.AuthType == y.AuthType && x.UserSettingsId == y.UserSettingsId;
    }

    public int GetHashCode(User obj)
    {
        return HashCode.Combine(obj.HexId, obj.GitHubId, obj.Username, (int)obj.AuthType, obj.UserSettingsId);
    }
}

public class UserSettingsEqualityComparer : IEqualityComparer<UserSettings>
{
    public bool Equals([AllowNull] UserSettings x, [AllowNull] UserSettings y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.UserId == y.UserId && x.StatisticsEnabled == y.StatisticsEnabled;
    }

    public int GetHashCode(UserSettings obj)
    {
        return HashCode.Combine(obj.UserId, obj.StatisticsEnabled);
    }
}

public class UserStatisticsEqualityComparer : IEqualityComparer<UserStatistics>
{
    public bool Equals([AllowNull] UserStatistics x, [AllowNull] UserStatistics y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.UserId == y.UserId && x.SecondsSpentInVoice == y.SecondsSpentInVoice && x.ReactionsSet == y.ReactionsSet && x.MessagesSent == y.MessagesSent && x.RoomsJoined == y.RoomsJoined && x.RoomsCreated == y.RoomsCreated;
    }

    public int GetHashCode(UserStatistics obj)
    {
        return HashCode.Combine(obj.UserId, obj.SecondsSpentInVoice, obj.ReactionsSet, obj.MessagesSent, obj.RoomsJoined, obj.RoomsCreated);
    }
}
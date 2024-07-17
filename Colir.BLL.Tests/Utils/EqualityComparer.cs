using System.Diagnostics.CodeAnalysis;
using Colir.BLL.Models;

namespace Colir.BLL.Tests.Utils;

class AttachmentModelEqualityComparer : IEqualityComparer<AttachmentModel>
{
    public bool Equals([AllowNull] AttachmentModel x, [AllowNull] AttachmentModel y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Filename == y.Filename && x.Path == y.Path && x.SizeInKb == y.SizeInKb;
    }

    public int GetHashCode(AttachmentModel obj)
    {
        return HashCode.Combine(obj.Filename, obj.Path, obj.SizeInKb);
    }
}

class MessageModelEqualityComparer : IEqualityComparer<MessageModel>
{
    public bool Equals([AllowNull] MessageModel x, [AllowNull] MessageModel y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Id == y.Id && x.RoomId == y.RoomId && x.AuthorHexId == y.AuthorHexId && x.PostDate.Equals(y.PostDate) && Nullable.Equals(x.EditDate, y.EditDate) && x.RepliedMessageId == y.RepliedMessageId;
    }

    public int GetHashCode(MessageModel obj)
    {
        return HashCode.Combine(obj.Id, obj.RoomId, obj.AuthorHexId, obj.PostDate, obj.EditDate, obj.RepliedMessageId);
    }
}

class ReactionModelEqualityComparer : IEqualityComparer<ReactionModel>
{
    public bool Equals([AllowNull] ReactionModel x, [AllowNull] ReactionModel y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Id == y.Id && x.Symbol == y.Symbol && x.AuthorHexId == y.AuthorHexId;
    }

    public int GetHashCode(ReactionModel obj)
    {
        return HashCode.Combine(obj.Id, obj.Symbol, obj.AuthorHexId);
    }
}

class RoomModelEqualityComparer : IEqualityComparer<RoomModel>
{
    public bool Equals([AllowNull] RoomModel x, [AllowNull] RoomModel y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Guid == y.Guid && x.Name == y.Name && Nullable.Equals(x.ExpiryDate, y.ExpiryDate);
    }

    public int GetHashCode(RoomModel obj)
    {
        return HashCode.Combine(obj.Guid, obj.Name, obj.ExpiryDate);
    }
}

class UserModelEqualityComparer : IEqualityComparer<UserModel>
{
    public bool Equals([AllowNull] UserModel x, [AllowNull] UserModel y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.HexId == y.HexId && x.HexId == y.HexId && x.Username == y.Username && x.AuthType == y.AuthType;
    }

    public int GetHashCode(UserModel obj)
    {
        return HashCode.Combine(obj.HexId, obj.HexId, obj.Username, (int)obj.AuthType);
    }
}

class DetailedUserModelEqualityComparer : IEqualityComparer<DetailedUserModel>
{
    public bool Equals([AllowNull] DetailedUserModel x, [AllowNull] DetailedUserModel y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Id == y.Id && x.HexId == y.HexId && x.Username == y.Username && x.AuthType == y.AuthType;
    }

    public int GetHashCode(DetailedUserModel obj)
    {
        return HashCode.Combine(obj.Id, obj.HexId, obj.Username, (int)obj.AuthType);
    }
}

class UserSettingsModelEqualityComparer : IEqualityComparer<UserSettingsModel>
{
    public bool Equals([AllowNull] UserSettingsModel x, [AllowNull] UserSettingsModel y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.StatisticsEnabled == y.StatisticsEnabled;
    }

    public int GetHashCode(UserSettingsModel obj)
    {
        return obj.StatisticsEnabled.GetHashCode();
    }
}

class UserStatisticsModelEqualityComparer : IEqualityComparer<UserStatisticsModel>
{
    public bool Equals([AllowNull] UserStatisticsModel x, [AllowNull] UserStatisticsModel y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.UserId == y.UserId && x.SecondsSpentInVoice == y.SecondsSpentInVoice && x.ReactionsSet == y.ReactionsSet && x.MessagesSent == y.MessagesSent && x.RoomsJoined == y.RoomsJoined && x.RoomsCreated == y.RoomsCreated;
    }

    public int GetHashCode(UserStatisticsModel obj)
    {
        return HashCode.Combine(obj.UserId, obj.SecondsSpentInVoice, obj.ReactionsSet, obj.MessagesSent, obj.RoomsJoined, obj.RoomsCreated);
    }
}
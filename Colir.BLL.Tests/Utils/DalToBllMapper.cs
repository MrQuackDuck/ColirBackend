using Colir.BLL.Models;
using DAL.Entities;

namespace Colir.BLL.Tests.Utils;

public static class DalToBllMapper
{
    public static AttachmentModel ToAttachmentModel(this Attachment attachment)
    {
        return new AttachmentModel
        {
            Filename = attachment.Filename,
            Path = attachment.Path,
            SizeInBytes = attachment.SizeInBytes
        };
    }

    public static UserModel ToUserModel(this User user)
    {
        return new UserModel
        {
            HexId = user.HexId,
            Username = user.Username,
            AuthType = user.AuthType
        };
    }

    public static DetailedUserModel ToDetailedUserModel(this User user)
    {
        return new DetailedUserModel
        {
            Id = user.Id,
            HexId = user.HexId,
            Username = user.Username,
            AuthType = user.AuthType,
            UserSettings = user.UserSettings.ToUserSettingsModel(),
            UserStatistics = user.UserStatistics.ToUserStatisticsModel()
        };
    }

    public static MessageModel ToMessageModel(this Message message)
    {
        return new MessageModel
        {
            Id = message.Id,
            RoomId = message.RoomId,
            AuthorHexId = message.Author!.HexId,
            PostDate = message.PostDate,
            EditDate = message.EditDate,
            Content = message.Content,
            RepliedMessageId = message.RepliedMessageId,
            Reactions = message.Reactions.Select(r => r.ToReactionModel()).ToList(),
            Attachments = message.Attachments.Select(a => a.ToAttachmentModel()).ToList()
        };
    }

    public static ReactionModel ToReactionModel(this Reaction reaction)
    {
        return new ReactionModel
        {
            Id = reaction.Id,
            Symbol = reaction.Symbol,
            AuthorHexId = reaction.Author.HexId
        };
    }

    public static RoomModel ToRoomModel(this Room room)
    {
        return new RoomModel
        {
            Guid = room.Guid,
            Name = room.Name,
            ExpiryDate = room.ExpiryDate,
            Owner = room.Owner.ToUserModel(),
            JoinedUsers = room.JoinedUsers.Select(u => u.ToUserModel()).ToList(),
            FreeMemoryInBytes = 0,
            UsedMemoryInBytes = 0
        };
    }

    public static UserSettingsModel ToUserSettingsModel(this UserSettings userSettings)
    {
        return new UserSettingsModel
        {
            StatisticsEnabled = userSettings.StatisticsEnabled
        };
    }

    public static UserStatisticsModel ToUserStatisticsModel(this UserStatistics userStatistics)
    {
        return new UserStatisticsModel
        {
            UserId = userStatistics.UserId,
            SecondsSpentInVoice = userStatistics.SecondsSpentInVoice,
            ReactionsSet = userStatistics.ReactionsSet,
            MessagesSent = userStatistics.MessagesSent,
            RoomsJoined = userStatistics.RoomsJoined,
            RoomsCreated = userStatistics.RoomsCreated
        };
    }
}
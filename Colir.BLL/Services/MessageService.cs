using AutoMapper;
using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.Message;
using Colir.BLL.RequestModels.Room;
using Colir.Exceptions;
using Colir.Exceptions.NotEnoughPermissions;
using Colir.Exceptions.NotFound;
using DAL.Entities;
using DAL.Interfaces;

namespace Colir.BLL.Services;

public class MessageService : IMessageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public MessageService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc cref="IMessageService.GetLastMessagesAsync"/>
    public async Task<List<MessageModel>> GetLastMessagesAsync(RequestToGetLastMessages request)
    {
        // Check if the issuer exists. Otherwise, an exception will be thrown
        await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);

        var room = await _unitOfWork.RoomRepository.GetByGuidAsync(request.RoomGuid);

        // If the room is expired
        if (room.IsExpired())
        {
            throw new RoomExpiredException();
        }

        // If the issuer is not in the room
        if (!room.JoinedUsers.Any(u => u.Id == request.IssuerId))
        {
            throw new IssuerNotInRoomException();
        }

        return (await _unitOfWork
            .MessageRepository
            .GetLastMessagesAsync(request.RoomGuid, request.Count, request.SkipCount))
            .Select(m =>
            {
                var mapped = _mapper.Map<MessageModel>(m);
                if (mapped.RepliedMessage != null && m.RepliedTo != null && m.RepliedTo.Author != null)
                {
                    mapped.RepliedMessage.AuthorHexId = m.RepliedTo.Author.HexId;
                }
                mapped.RoomGuid = room.Guid;
                return mapped;
            })
            .ToList();
    }

    /// <inheritdoc cref="IMessageService.GetSurroundingMessagesAsync"/>
    public async Task<List<MessageModel>> GetSurroundingMessagesAsync(RequestToGetSurroundingMessages request)
    {
        // Check if the issuer exists. Otherwise, an exception will be thrown
        await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);

        var message = await _unitOfWork.MessageRepository.GetByIdAsync(request.MessageId);

        var room = await _unitOfWork.RoomRepository.GetByIdAsync(message.RoomId);

        // If the room is expired
        if (room.IsExpired())
        {
            throw new RoomExpiredException();
        }

        // If the issuer is not in the room
        if (!room.JoinedUsers.Any(u => u.Id == request.IssuerId))
        {
            throw new IssuerNotInRoomException();
        }

        return (await _unitOfWork
            .MessageRepository
            .GetSurroundingMessages(room.Guid, request.MessageId, request.Count))
            .Select(m =>
            {
                var mapped = _mapper.Map<MessageModel>(m);
                mapped.RoomGuid = room.Guid;
                if (mapped.RepliedMessage != null && m.RepliedTo != null && m.RepliedTo.Author != null)
                {
                    mapped.RepliedMessage.AuthorHexId = m.RepliedTo.Author.HexId;
                }
                return mapped;
            })
            .ToList();
    }

    /// <inheritdoc cref="IMessageService.GetMessagesRangeAsync"/>
    public async Task<List<MessageModel>> GetMessagesRangeAsync(RequestToGetMessagesRange request)
    {
        if (request.StartId < 0 || request.EndId < 0)
        {
            throw new ArgumentException("Ids must be positive");
        }

        // Check if the issuer exists. Otherwise, an exception will be thrown
        await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);

        var room = await _unitOfWork.RoomRepository.GetByGuidAsync(request.RoomGuid);

        // If the room is expired
        if (room.IsExpired())
        {
            throw new RoomExpiredException();
        }

        // If the issuer is not in the room
        if (!room.JoinedUsers.Any(u => u.Id == request.IssuerId))
        {
            throw new IssuerNotInRoomException();
        }

        var startMessage = await _unitOfWork.MessageRepository.GetByIdAsync(request.StartId);
        var endMessage = await _unitOfWork.MessageRepository.GetByIdAsync(request.EndId);

        if (startMessage.RoomId != room.Id || endMessage.RoomId != room.Id)
        {
            throw new MessageNotFoundException();
        }

        return (await _unitOfWork
            .MessageRepository
            .GetMessagesRangeAsync(request.RoomGuid, request.StartId, request.EndId))
            .Select(m =>
            {
                var mapped = _mapper.Map<MessageModel>(m);
                mapped.RoomGuid = room.Guid;
                if (mapped.RepliedMessage != null && m.RepliedTo != null && m.RepliedTo.Author != null)
                {
                    mapped.RepliedMessage.AuthorHexId = m.RepliedTo.Author.HexId;
                }
                return mapped;
            })
            .ToList();
    }

    /// <inheritdoc cref="IMessageService.GetUnreadRepliesAsync"/>
    public async Task<List<MessageModel>> GetUnreadRepliesAsync(RequestToGetUnreadReplies request)
    {
        // Check if the issuer exists. Otherwise, an exception will be thrown
        await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);

        var room = await _unitOfWork.RoomRepository.GetByGuidAsync(request.RoomGuid);

        // If the room is expired
        if (room.IsExpired())
        {
            throw new RoomExpiredException();
        }

        // If the issuer is not in the room
        if (!room.JoinedUsers.Any(u => u.Id == request.IssuerId))
        {
            throw new IssuerNotInRoomException();
        }

        LastTimeUserReadChat lastTimeUserReadChat;
        try
        {
            lastTimeUserReadChat = await _unitOfWork
                .LastTimeUserReadChatRepository
                .GetAsync(request.IssuerId, room.Id);
        }
        // If not found, create
        catch (NotFoundException)
        {
            lastTimeUserReadChat = new LastTimeUserReadChat
            {
                UserId = request.IssuerId,
                RoomId = room.Id,
                Timestamp = DateTime.Now
            };

            await _unitOfWork.LastTimeUserReadChatRepository.AddAsync(lastTimeUserReadChat);
            await _unitOfWork.SaveChangesAsync();
        }

        return (await _unitOfWork
            .MessageRepository
            .GetAllRepliesToUserAfterDateAsync(request.RoomGuid, request.IssuerId, lastTimeUserReadChat.Timestamp))
            .Select(m =>
            {
                var mapped = _mapper.Map<MessageModel>(m);
                mapped.RoomGuid = room.Guid;
                if (mapped.RepliedMessage != null && m.RepliedTo != null && m.RepliedTo.Author != null)
                {
                    mapped.RepliedMessage.AuthorHexId = m.RepliedTo.Author.HexId;
                }
                return mapped;
            }).ToList();
    }

    /// <inheritdoc cref="IMessageService.GetMessageById"/>
    public async Task<MessageModel> GetMessageById(RequestToGetMessage request)
    {
        // Check if the issuer exists. Otherwise, an exception will be thrown
        await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);

        var messageToGet = await _unitOfWork.MessageRepository.GetByIdAsync(request.MessageId);

        var room = await _unitOfWork.RoomRepository.GetByIdAsync(messageToGet.RoomId);

        // If the room is expired
        if (room.IsExpired())
        {
            throw new RoomExpiredException();
        }

        // If the issuer is not in the room
        if (!room.JoinedUsers.Any(u => u.Id == request.IssuerId))
        {
            throw new IssuerNotInRoomException();
        }

        return _mapper.Map<MessageModel>(messageToGet);
    }

    /// <inheritdoc cref="IMessageService.SendAsync"/>
    public async Task<MessageModel> SendAsync(RequestToSendMessage request)
    {
        // Check if the message is not empty
        if (request.Content.Length == 0 && (request.AttachmentsIds == null || request.AttachmentsIds.Count == 0))
        {
            throw new ArgumentException("Message can't be empty!");
        }

        var issuer = await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);

        var room = await _unitOfWork.RoomRepository.GetByGuidAsync(request.RoomGuid);

        // If the room is expired
        if (room.IsExpired())
        {
            throw new RoomExpiredException();
        }

        // If the issuer is not in the room
        if (!room.JoinedUsers.Any(u => u.Id == request.IssuerId))
        {
            throw new IssuerNotInRoomException();
        }

        Message repliedMessage = null!;

        // If the ReplyMessageId is not null, check if the message to reply exists
        // Otherwise, an exception will be thrown
        if (request.ReplyMessageId is not null)
        {
            repliedMessage = await _unitOfWork.MessageRepository.GetByIdAsync(request.ReplyMessageId ?? -1);
        }

        var messageToSend = new Message
        {
            Content = request.Content,
            PostDate = DateTime.Now,
            RoomId = room.Id,
            AuthorId = request.IssuerId,
            RepliedMessageId = request.ReplyMessageId
        };

        var transaction = _unitOfWork.BeginTransaction();

        await _unitOfWork.MessageRepository.AddAsync(messageToSend);

        if (request.AttachmentsIds != null!)
        {
            // Adding attachments to the message
            foreach (var attachmentId in request.AttachmentsIds)
            {
                var attachment = await _unitOfWork.AttachmentRepository.GetByIdAsync(attachmentId);

                // Verifying that attachment is in the room the message is being sent to
                if (!attachment.IsInRoom(request.RoomGuid))
                {
                    throw new AttachmentNotFoundException();
                }

                attachment.Message = messageToSend;
                _unitOfWork.AttachmentRepository.Update(attachment);
            }
        }

        if (issuer.UserSettings.StatisticsEnabled)
        {
            // Adding the info about the sent message to statistics
            issuer.UserStatistics.MessagesSent += 1;
            _unitOfWork.UserStatisticsRepository.Update(issuer.UserStatistics);
        }

        try
        {
            await _unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        var result = _mapper.Map<MessageModel>(messageToSend);
        result.AuthorHexId = issuer.HexId;
        if (repliedMessage != null)
        {
            result.RepliedMessage = _mapper.Map<MessageModel>(repliedMessage);
        }

        return result;
    }

    /// <inheritdoc cref="IMessageService.EditAsync"/>
    public async Task<MessageModel> EditAsync(RequestToEditMessage request)
    {
        // Check if the issuer exists. Otherwise, an exception will be thrown
        await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);

        var message = await _unitOfWork.MessageRepository.GetByIdAsync(request.MessageId);

        // Check if not empty
        if (request.NewContent.Length == 0 && message.Attachments.Count == 0)
        {
            throw new ArgumentException("Message content can't be empty!");
        }

        var room = await _unitOfWork.RoomRepository.GetByIdAsync(message.RoomId);

        // If the room is expired
        if (room.IsExpired())
        {
            throw new RoomExpiredException();
        }

        // If the issuer is not in the room
        if (!room.JoinedUsers.Any(u => u.Id == request.IssuerId))
        {
            throw new IssuerNotInRoomException();
        }

        // If the issuer is not the author of the message
        if (message.AuthorId != request.IssuerId)
        {
            throw new NotEnoughPermissionsException();
        }

        message.Content = request.NewContent;
        _unitOfWork.MessageRepository.Update(message);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<MessageModel>(message);
    }

    /// <inheritdoc cref="IMessageService.DeleteAsync"/>
    public async Task DeleteAsync(RequestToDeleteMessage request)
    {
        // Check if the issuer exists. Otherwise, an exception will be thrown
        await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);

        var message = await _unitOfWork.MessageRepository.GetByIdAsync(request.MessageId);
        var room = await _unitOfWork.RoomRepository.GetByIdAsync(message.RoomId);

        // If the room is expired
        if (room.IsExpired())
        {
            throw new RoomExpiredException();
        }

        // If the issuer is not in the room
        if (!room.JoinedUsers.Any(u => u.Id == request.IssuerId))
        {
            throw new IssuerNotInRoomException();
        }

        // If the issuer is not the author of the message
        if (message.AuthorId != request.IssuerId)
        {
            throw new NotEnoughPermissionsException();
        }

        var transaction = _unitOfWork.BeginTransaction();

        // Deleting the attachments from the file system
        foreach (var attachment in message.Attachments)
        {
            _unitOfWork.RoomRepository.RoomFileManager.DeleteFile(attachment.Path);
        }

        _unitOfWork.MessageRepository.Delete(message);

        try
        {
            await _unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <inheritdoc cref="IMessageService.AddReaction"/>
    public async Task<MessageModel> AddReaction(RequestToAddReactionOnMessage request)
    {
        var issuer = await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);
        var message = await _unitOfWork.MessageRepository.GetByIdAsync(request.MessageId);
        var room = await _unitOfWork.RoomRepository.GetByIdAsync(message.RoomId);

        // If the reaction is already set
        if (message.Reactions.Exists(r => r.AuthorId == request.IssuerId && r.Symbol == request.Reaction))
        {
            throw new InvalidActionException();
        }

        // If the issuer is not in the room
        if (!room.JoinedUsers.Any(u => u.Id == request.IssuerId))
        {
            throw new IssuerNotInRoomException();
        }

        // If the room is expired
        if (room.IsExpired())
        {
            throw new RoomExpiredException();
        }

        var reactionToAdd = new Reaction
        {
            AuthorId = issuer.Id,
            MessageId = message.Id,
            Symbol = request.Reaction
        };

        var transaction = _unitOfWork.BeginTransaction();
        try
        {
            if (issuer.UserSettings.StatisticsEnabled)
            {
                // Adding the info about sent message to statistics
                issuer.UserStatistics.ReactionsSet += 1;
                _unitOfWork.UserStatisticsRepository.Update(issuer.UserStatistics);
            }

            await _unitOfWork.ReactionRepository.AddAsync(reactionToAdd);
            await _unitOfWork.SaveChangesAsync();

            message.Reactions.Add(reactionToAdd);
            _unitOfWork.MessageRepository.Update(message);

            await _unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        reactionToAdd.Author = issuer;
        return _mapper.Map<MessageModel>(message);
    }

    /// <inheritdoc cref="IMessageService.RemoveReaction"/>
    public async Task<MessageModel> RemoveReaction(RequestToRemoveReactionFromMessage request)
    {
        var issuer = await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);
        var reaction = await _unitOfWork.ReactionRepository.GetByIdAsync(request.ReactionId);
        var message = await _unitOfWork.MessageRepository.GetByIdAsync(reaction.MessageId);
        var room = await _unitOfWork.RoomRepository.GetByIdAsync(reaction.Message.RoomId);

        // If the room is expired
        if (room.IsExpired())
        {
            throw new RoomExpiredException();
        }

        // If the issuer is not in the room
        if (!room.JoinedUsers.Any(u => u.Id == request.IssuerId))
        {
            throw new IssuerNotInRoomException();
        }

        // If the issuer is not the author of the reaction
        if (issuer.Id != reaction.AuthorId)
        {
            throw new NotEnoughPermissionsException();
        }

        var transaction = _unitOfWork.BeginTransaction();
        try
        {
            if (issuer.UserSettings.StatisticsEnabled)
            {
                // Adding the info about sent message to statistics
                issuer.UserStatistics.ReactionsSet += 1;
                _unitOfWork.UserStatisticsRepository.Update(issuer.UserStatistics);
            }

            message.Reactions.Remove(message.Reactions.First(r => r.Id == reaction.Id));

            await _unitOfWork.ReactionRepository.DeleteByIdAsync(reaction.Id);
            await _unitOfWork.SaveChangesAsync();

            await transaction.CommitAsync();

            return _mapper.Map<MessageModel>(message);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
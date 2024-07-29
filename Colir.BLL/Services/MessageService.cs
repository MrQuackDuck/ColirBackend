﻿using AutoMapper;
using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.Message;
using Colir.Exceptions;
using Colir.Exceptions.NotEnoughPermissions;
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
            .GetLastMessages(request.RoomGuid, request.Count, request.SkipCount))
            .Select(m => _mapper.Map<MessageModel>(m))
            .ToList();
    }
    
    /// <inheritdoc cref="IMessageService.SendAsync"/>
    public async Task<MessageModel> SendAsync(RequestToSendMessage request)
    {
        // Check if not empty
        if (request.Content.Length == 0)
        {
            throw new ArgumentException("Message content can't be empty!");
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

        // If the ReplyMessageId is not null, check if the message to reply exists
        // Otherwise, an exception will be thrown
        if (request.ReplyMessageId is not null)
        {
            await _unitOfWork.MessageRepository.GetByIdAsync(request.ReplyMessageId ?? -1);
        }
        
        var transaction = _unitOfWork.BeginTransaction();
        
        var messageToSend = new Message
        {
            Content = request.Content,
            PostDate = DateTime.Now,
            RoomId = room.Id,
            AuthorId = request.IssuerId,
            RepliedMessageId = request.ReplyMessageId
        };

        await _unitOfWork.MessageRepository.AddAsync(messageToSend);
        
        if (request.AttachmentsIds != null!)
        {
            // Adding attachments to the message
            foreach (var attachmentId in request.AttachmentsIds)
            {
                var attachment = await _unitOfWork.AttachmentRepository.GetByIdAsync(attachmentId);
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
        
        await _unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();

        return _mapper.Map<MessageModel>(messageToSend);
    }

    /// <inheritdoc cref="IMessageService.EditAsync"/>
    public async Task<MessageModel> EditAsync(RequestToEditMessage request)
    {
        // Check if not empty
        if (request.NewContent.Length == 0)
        {
            throw new ArgumentException("Message content can't be empty!");
        }
        
        // Check if the issuer exists. Otherwise, an exception will be thrown
        await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);

        var message = await _unitOfWork.MessageRepository.GetByIdAsync(request.MessageId);
        var room = await _unitOfWork.RoomRepository.GetByIdAsync(message.RoomId ?? 0);
        
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

        message.Content = request.NewContent;
        _unitOfWork.MessageRepository.Update(message);
        
        await _unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();
        
        return _mapper.Map<MessageModel>(message);
    }

    /// <inheritdoc cref="IMessageService.DeleteAsync"/>
    public async Task DeleteAsync(RequestToDeleteMessage request)
    {
        // Check if the issuer exists. Otherwise, an exception will be thrown
        await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);

        var message = await _unitOfWork.MessageRepository.GetByIdAsync(request.MessageId);
        var room = await _unitOfWork.RoomRepository.GetByIdAsync(message.RoomId ?? 0);
        
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

        _unitOfWork.MessageRepository.Delete(message);
        
        await _unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    /// <inheritdoc cref="IMessageService.AddReaction"/>
    public async Task<MessageModel> AddReaction(RequestToAddReactionOnMessage request)
    {
        var issuer = await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);
        var message = await _unitOfWork.MessageRepository.GetByIdAsync(request.MessageId);
        var room = await _unitOfWork.RoomRepository.GetByIdAsync(message.RoomId ?? 0);
        
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
        
        var transaction = _unitOfWork.BeginTransaction();
        
        var reactionToAdd = new Reaction
        {
            AuthorId = request.IssuerId,
            MessageId = message.Id,
            Symbol = request.Reaction
        };
        
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
        
        return _mapper.Map<MessageModel>(message);
    }

    /// <inheritdoc cref="IMessageService.RemoveReaction"/>
    public async Task<MessageModel> RemoveReaction(RequestToRemoveReactionFromMessage request)
    {
        var issuer = await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);
        var reaction = await _unitOfWork.ReactionRepository.GetByIdAsync(request.ReactionId);
        var message = await _unitOfWork.MessageRepository.GetByIdAsync(reaction.MessageId);
        var room = await _unitOfWork.RoomRepository.GetByIdAsync(reaction.Message.RoomId ?? 0);
        
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
        
        if (issuer.UserSettings.StatisticsEnabled)
        {
            // Adding the info about sent message to statistics
            issuer.UserStatistics.ReactionsSet += 1;
            _unitOfWork.UserStatisticsRepository.Update(issuer.UserStatistics);
        }

        await _unitOfWork.ReactionRepository.DeleteByIdAsync(reaction.Id);
        await _unitOfWork.SaveChangesAsync();
        
        await _unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();
        
        return _mapper.Map<MessageModel>(message);
    }
}
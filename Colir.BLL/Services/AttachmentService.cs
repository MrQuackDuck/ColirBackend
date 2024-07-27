using AutoMapper;
using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.Attachment;
using Colir.Exceptions;
using Colir.Exceptions.NotEnoughPermissions;
using DAL.Entities;
using DAL.Interfaces;

namespace Colir.BLL.Services;

public class AttachmentService : IAttachmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    
    public AttachmentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    /// <inheritdoc cref="IAttachmentService.UploadAttachmentAsync"/>
    public async Task<AttachmentModel> UploadAttachmentAsync(RequestToUploadAttachment request)
    {
        var issuer = await _unitOfWork.UserRepository.GetByIdAsync(request.IssuerId);
        var room = await _unitOfWork.RoomRepository.GetByGuidAsync(request.RoomGuid);
        
        // Check if room's expired
        if (room.IsExpired())
        {
            throw new RoomExpiredException();
        }
        
        // Check if the issuer's in the room
        if (!room.JoinedUsers.Any(u => u.Id == issuer.Id))
        {
            throw new IssuerNotInRoomException();
        }

        // Check if enough free space is available
        if (request.File.Length > _unitOfWork.RoomRepository.RoomFileManager.GetFreeStorageSize(room.Guid))
        {
            throw new ArgumentException("No more room storage left!");
        }
        
        var path = await _unitOfWork.RoomRepository.RoomFileManager.UploadFileAsync(room.Guid, request.File);

        var attachment = new Attachment
        {
            Filename = request.File.FileName,
            Path = path,
            SizeInBytes = request.File.Length,
        };

        var transaction = _unitOfWork.BeginTransaction();

        // Adding the attachment to DB
        await _unitOfWork.AttachmentRepository.AddAsync(attachment);
        
        await _unitOfWork.SaveChangesAsync();
        await transaction.CommitAsync();

        return _mapper.Map<AttachmentModel>(attachment);
    }
}
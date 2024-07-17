﻿using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.Attachment;
using DAL.Interfaces;

namespace Colir.BLL.Services;

public class AttachmentService : IAttachmentService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public AttachmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<AttachmentModel> UploadAttachmentAsync(RequestToUploadAttachment request)
    {
        throw new NotImplementedException();
    }
}
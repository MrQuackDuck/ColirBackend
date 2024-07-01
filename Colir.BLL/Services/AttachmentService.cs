using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.Attachment;
using DAL.Interfaces;

namespace Colir.BLL.Services;

public class AttachmentService : IAttachmentService
{
    private IUnitOfWork _unitOfWork;
    
    public AttachmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public AttachmentModel UploadAttachment(RequestToUploadAttachment request)
    {
        throw new NotImplementedException();
    }
}
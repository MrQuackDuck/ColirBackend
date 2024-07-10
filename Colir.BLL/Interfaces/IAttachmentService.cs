using Colir.BLL.Models;
using Colir.BLL.RequestModels.Attachment;

namespace Colir.BLL.Interfaces;

public interface IAttachmentService
{
    Task<AttachmentModel> UploadAttachmentAsync(RequestToUploadAttachment request);
}
using Colir.BLL.Models;
using Colir.BLL.RequestModels.Attachment;

namespace Colir.BLL.Interfaces;

public interface IAttachmentService
{
    AttachmentModel UploadAttachment(RequestToUploadAttachment request);
}
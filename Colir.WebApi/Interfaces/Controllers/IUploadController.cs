using Colir.Communication.RequestModels.Upload;
using Microsoft.AspNetCore.Mvc;

namespace Colir.Interfaces.Controllers;

public interface IUploadController
{
    /// <summary>
    /// Uploads an attachment and returns a list of attachment ids
    /// </summary>
    Task<ActionResult<List<long>>> UploadAttachments(UploadAttachmentsModel model);
}
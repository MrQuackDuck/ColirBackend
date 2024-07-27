using Colir.BLL.Models;
using Colir.BLL.RequestModels.Attachment;
using Colir.Exceptions;
using Colir.Exceptions.NotEnoughPermissions;
using Colir.Exceptions.NotFound;

namespace Colir.BLL.Interfaces;

public interface IAttachmentService
{
    /// <summary>
    /// Uploads an attachment
    /// </summary>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    /// <exception cref="RoomNotFoundException">Thrown when the wasn't found</exception>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when the issuer is not in the room</exception>
    /// <exception cref="ArgumentException">Thrown when no free space left</exception>
    Task<AttachmentModel> UploadAttachmentAsync(RequestToUploadAttachment request);
}
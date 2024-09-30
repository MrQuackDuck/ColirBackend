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
    /// <exception cref="ArgumentException">Thrown when no free space left</exception>
    /// <exception cref="UserNotFoundException">Thrown when the issuer wasn't found</exception>
    /// <exception cref="RoomNotFoundException">Thrown when the wasn't found</exception>
    /// <exception cref="RoomExpiredException">Thrown when the room is expired</exception>
    /// <exception cref="IssuerNotInRoomException">Thrown when the issuer is not in the room</exception>
    Task<AttachmentModel> UploadAttachmentAsync(RequestToUploadAttachment request);

    /// <summary>
    /// Checks if the attachment is attached to any message
    /// </summary>
    /// <param name="attachmentId">Id of the attachment</param>
    /// <returns>True if the attachment is attached to any message, false otherwise</returns>
    /// <exception cref="AttachmentNotFoundException">Thrown when the attachment wasn't found</exception>
    Task<bool> CheckIfAttachmentIsAttachedToAnyMessageAsync(long attachmentId);
}
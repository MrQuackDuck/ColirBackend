namespace Colir.BLL.Tests.Interfaces;

public interface IAttachmentServiceTests
{
    Task UploadAttachmentAsync_ThrowsUserNotFoundException_WhenIssuerNotFound();
    Task UploadAttachmentAsync_ThrowsRoomExpiredException_WhenRoomIsExpired();
    Task UploadAttachmentAsync_ThrowsRoomNotFoundException_WhenRoomNotFound();
    Task UploadAttachmentAsync_ThrowsIssuerNotInRoomExceptionException_WhenIssuerNotInRoom();
}
namespace Colir.BLL.Tests.Interfaces;

public interface IAttachmentServiceTests
{
    Task UploadAttachment_UploadsAttachment();
    Task UploadAttachment_AttachemtnHasTheSameSize();
    Task UploadAttachment_AttachmentIsAccesibleByPath();
    Task UploadAttachment_ThrowsRoomNotFoundException_WhenRoomNotFound();
}
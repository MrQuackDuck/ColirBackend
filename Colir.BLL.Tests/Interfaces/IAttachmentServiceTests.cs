namespace Colir.BLL.Tests.Interfaces;

public interface IAttachmentServiceTests
{
    void UploadAttachment_UploadsAttachment();
    void UploadAttachment_AttachemtnHasTheSameSize();
    void UploadAttachment_AttachmentIsAccesibleByPath();
    void UploadAttachment_ThrowsRoomNotFoundException_WhenRoomNotFound();
}
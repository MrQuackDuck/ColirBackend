namespace Colir.HttpModels.Message;

public class UploadAttachmentModel
{
    public string RoomGuid { get; set; } = default!;
    public IFormFile File { get; set; } = default!;
}
namespace Colir.Communication.RequestModels.Upload;

public class UploadAttachmentsModel
{
    public string RoomGuid { get; set; } = default!;
    public List<IFormFile> Files { get; set; } = default!;
}
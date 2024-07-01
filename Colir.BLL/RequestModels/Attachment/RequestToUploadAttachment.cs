using Microsoft.AspNetCore.Http;

namespace Colir.BLL.RequestModels.Attachment;

public class RequestToUploadAttachment
{
    public long IssuerId { get; set; }
    public string RoomGuid { get; set; } = default!;
    public IFormFile File { get; set; } = default!;
}
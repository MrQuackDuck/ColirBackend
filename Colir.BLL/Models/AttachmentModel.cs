using DAL.Enums;

namespace Colir.BLL.Models;

public class AttachmentModel
{
    public string Filename { get; set; } = default!;
    public string Path { get; set; } = default!;
    public AttachmentType AttachmentType;
    public long SizeInKb { get; set; }
}
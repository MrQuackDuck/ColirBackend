namespace Colir.BLL.Models;

public class AttachmentModel
{
    public string Filename { get; set; } = default!;
    public string Path { get; set; } = default!;
    public long SizeInBytes { get; set; }
}
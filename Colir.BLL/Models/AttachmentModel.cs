namespace Colir.BLL.Models;

public class AttachmentModel
{
    public long Id { get; set; } = default!;
    public string Filename { get; set; } = default!;
    public string Path { get; set; } = default!;
    public long SizeInBytes { get; set; }
}
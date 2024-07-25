namespace Colir.HttpModels.Chat;

public class DeleteMessageModel
{
    public long MessageId { get; set; }
    public string NewContent { get; set; } = default!;
}
namespace Colir.HttpModels.Chat;

public class EditMessageModel
{
    public long MessageId { get; set; } = default!;
    public string NewContent { get; set; } = default!;
}
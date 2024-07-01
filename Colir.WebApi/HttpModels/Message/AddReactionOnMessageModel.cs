namespace Colir.HttpModels.Message;

public class AddReactionOnMessageModel
{
    public long MessageId { get; set; }
    public string Reaction { get; set; } = default!;
}
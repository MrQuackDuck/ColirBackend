namespace Colir.Communication.RequestModels.Chat;

public class AddReactionOnMessageModel
{
    public long MessageId { get; set; }
    public string Reaction { get; set; } = default!;
}
namespace Colir.Communication.RequestModels.Chat;

public class GetMessagesRangeModel
{
    public long StartId { get; set; }
    public long EndId { get; set; }
}
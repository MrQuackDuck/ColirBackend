using Colir.Communication.RequestModels.Chat;

namespace Colir.Interfaces.Hubs;

public interface IChatHub
{
    Task GetMessages(GetLastMessagesModel model);
    Task SendMessage(SendMessageModel model);
    Task EditMessage(EditMessageModel model);
    Task DeleteMessage(DeleteMessageModel model);
    Task AddReactionOnMessage(AddReactionOnMessageModel model);
    Task RemoveReactionFromMessage(RemoveReactionFromMessageModel model);
}
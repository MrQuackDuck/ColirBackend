using Colir.BLL.Models;
using Colir.HttpModels.Chat;

namespace Colir.Interfaces.Hubs;

public interface IChatHub
{
    Task Connect(string roomGuid);
    Task<List<MessageModel>> GetMessages(GetLastMessagesModel model);
    Task<MessageModel> SendMessage(SendMessageModel model);
    Task<MessageModel> EditMessage(EditMessageModel model);
    Task DeleteMessage(DeleteMessageModel model);
    Task<MessageModel> AddReactionOnMessage(AddReactionOnMessageModel model);
    Task<MessageModel> RemoveReactionFromMessage(RemoveReactionFromMessageModel model);
}
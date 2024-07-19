using Colir.BLL.Models;
using Colir.BLL.RequestModels.Message;

namespace Colir.BLL.Interfaces;

public interface IMessageService
{
    Task<List<MessageModel>> GetLastMessagesAsync(RequestToGetLastMessages request);
    Task<MessageModel> SendAsync(RequestToSendMessage request); // - Returns a new message
    Task<MessageModel> EditAsync(RequestToEditMessage request); // - Returns edited message
    Task DeleteAsync(RequestToDeleteMessage request); // (!) Also will delete attached files

    Task<MessageModel> AddReaction(RequestToAddReactionOnMessage request); // - Returns a message with reaction
    Task<MessageModel> RemoveReaction(RequestToRemoveReactionFromMessage request); // - Returns a message without reaction
}
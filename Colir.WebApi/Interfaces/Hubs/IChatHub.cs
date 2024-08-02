using Colir.Communication.RequestModels.Chat;
using Colir.Communication.ResponseModels;

namespace Colir.Interfaces.Hubs;

public interface IChatHub
{
    Task<SignalRHubResult> GetMessages(GetLastMessagesModel model);
    Task<SignalRHubResult> SendMessage(SendMessageModel model);
    Task<SignalRHubResult> EditMessage(EditMessageModel model);
    Task<SignalRHubResult> DeleteMessage(DeleteMessageModel model);
    Task<SignalRHubResult> AddReactionOnMessage(AddReactionOnMessageModel model);
    Task<SignalRHubResult> RemoveReactionFromMessage(RemoveReactionFromMessageModel model);
}
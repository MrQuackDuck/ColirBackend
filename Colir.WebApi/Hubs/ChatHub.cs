using Colir.BLL.Models;
using Colir.Communication.RequestModels.Chat;
using Colir.Interfaces.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Colir.Hubs;

[Authorize]
public class ChatHub : Hub<IChatHub>
{
    public async Task Connect(string roomGuid)
    {
        throw new NotImplementedException();
    }

    public async Task<List<MessageModel>> GetMessages(GetLastMessagesModel model)
    {
        throw new NotImplementedException();
    }

    public async Task<MessageModel> SendMessage(SendMessageModel model)
    {
        throw new NotImplementedException();
    }

    public async Task<MessageModel> EditMessage(EditMessageModel model)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteMessage(DeleteMessageModel model)
    {
        throw new NotImplementedException();
    }

    public async Task<MessageModel> AddReactionOnMessage(AddReactionOnMessageModel model)
    {
        throw new NotImplementedException();
    }

    public async Task<MessageModel> RemoveReactionFromMessage(RemoveReactionFromMessageModel model)
    {
        throw new NotImplementedException();
    }
}
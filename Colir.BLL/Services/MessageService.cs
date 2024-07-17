using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.Message;
using DAL.Interfaces;

namespace Colir.BLL.Services;

public class MessageService : IMessageService
{
    private readonly IUnitOfWork _unitOfWork;
    
    public MessageService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<List<MessageModel>> GetLastMessagesAsync(RequestToGetLastMessages request)
    {
        throw new NotImplementedException();
    }

    public async Task<MessageModel> SendAsync(RequestToSendMessage request)
    {
        throw new NotImplementedException();
    }

    public async Task<MessageModel> EditAsync(RequestToEditMessage request)
    {
        throw new NotImplementedException();
    }

    public void Delete(RequestToDeleteMessage request)
    {
        throw new NotImplementedException();
    }

    public async Task<MessageModel> AddReaction(RequestToAddReactionOnMessage request)
    {
        throw new NotImplementedException();
    }

    public async Task<MessageModel> RemoveReaction(RequestToRemoveReactionFromMessage request)
    {
        throw new NotImplementedException();
    }
}
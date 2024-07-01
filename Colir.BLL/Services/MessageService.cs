using Colir.BLL.Interfaces;
using Colir.BLL.Models;
using Colir.BLL.RequestModels.Message;
using DAL.Interfaces;

namespace Colir.BLL.Services;

public class MessageService : IMessageService
{
    private IUnitOfWork _unitOfWork;
    
    public MessageService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public Task<List<MessageModel>> GetLastMessagesAsync(RequestToGetLastMessages request)
    {
        throw new NotImplementedException();
    }

    public Task<MessageModel> SendAsync(RequestToSendMessage request)
    {
        throw new NotImplementedException();
    }

    public Task<MessageModel> EditAsync(RequestToEditMessage request)
    {
        throw new NotImplementedException();
    }

    public void Delete(RequestToDeleteMessage request)
    {
        throw new NotImplementedException();
    }

    public Task<MessageModel> AddReaction(RequestToAddReactionOnMessage request)
    {
        throw new NotImplementedException();
    }

    public Task<MessageModel> RemoveReaction(RequestToRemoveReactionFromMessage request)
    {
        throw new NotImplementedException();
    }
}
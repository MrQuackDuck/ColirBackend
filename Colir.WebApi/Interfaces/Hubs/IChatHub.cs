using Colir.BLL.Models;
using Colir.HttpModels.Message;

namespace Colir.Interfaces.Hubs;

public interface IChatHub
{
    Task Connect(string roomGuid);
    Task<List<MessageModel>> GetMessages(GetLastMessagesModel model);
    Task<MessageModel> SendMessage(SendMessageModel model); // - Clients will receive MessageModel object
    Task<MessageModel> EditMessage(EditMessageModel model);
    Task DeleteMessage(DeleteMessageModel model);
    Task<MessageModel> AddReactionOnMessage(AddReactionOnMessageModel model);
    Task<MessageModel> RemoveReactionFromMessage(RemoveReactionFromMessageModel model);
    Task<AttachmentModel> UploadAttachment(UploadAttachmentModel model);
}
namespace DAL.Interfaces;

public interface IUnitOfWork
{
    IAttachmentRepository AttachmentRepository { get; }
    IRoomRepository RoomRepository { get; }
    IReactionRepository ReactionRepository { get; }
    IMessageRepository MessageRepository { get; }
    IUserRepository UserRepository { get; }
    IUserStatisticsRepository UserStatisticsRepository { get; }
    IUserSettingsRepository UserSettingsRepository { get; }
    ILastTimeUserReadChatRepository LastTimeUserReadChatRepository { get; }
    void SaveChanges();
}
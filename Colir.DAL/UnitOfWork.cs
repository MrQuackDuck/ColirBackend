using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.Extensions.Configuration;

namespace DAL;

public class UnitOfWork : IUnitOfWork
{
    private ColirDbContext _dbContext;
    
    private IAttachmentRepository _attachmentRepository;
    private IRoomRepository _roomRepository;
    private IReactionRepository _reactionRepository;
    private IMessageRepository _messageRepository;
    private IUserRepository _userRepository;
    private IUserStatisticsRepository _userStatisticsRepository;
    private IUserSettingsRepository _userSettingsRepository;
    private ILastTimeUserReadChatRepository _lastTimeUserReadChatRepository;
    private IConfiguration _configuration;

    public UnitOfWork(ColirDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }
    
    public IAttachmentRepository AttachmentRepository
    {
        get
        {
            if (this._attachmentRepository == null)
            {
                this._attachmentRepository = new AttachmentRepository(_dbContext);
            }
            
            return _attachmentRepository;
        }
    }
    
    public IRoomRepository RoomRepository
    {
        get
        {
            if (this._roomRepository == null)
            {
                this._roomRepository = new RoomRepository(_dbContext, _configuration);
            }
            
            return _roomRepository;
        }
    }

    public IReactionRepository ReactionRepository
    {
        get
        {
            if (this._reactionRepository == null)
            {
                this._reactionRepository = new ReactionRepository(_dbContext);
            }
            
            return _reactionRepository;
        }
    }

    public IMessageRepository MessageRepository
    {
        get
        {
            if (this._messageRepository == null)
            {
                this._messageRepository = new MessageRepository(_dbContext);
            }
            
            return _messageRepository;
        }
    }

    public IUserRepository UserRepository
    {
        get
        {
            if (this._userRepository == null)
            {
                this._userRepository = new UserRepository(_dbContext, _configuration);
            }
            
            return _userRepository;
        }
    }

    public IUserStatisticsRepository UserStatisticsRepository
    {
        get
        {
            if (this._userStatisticsRepository == null)
            {
                this._userStatisticsRepository = new UserStatisticsRepository(_dbContext);
            }
            
            return _userStatisticsRepository;
        }
    }

    public IUserSettingsRepository UserSettingsRepository
    {
        get
        {
            if (this._userSettingsRepository == null)
            {
                this._userSettingsRepository = new UserSettingsRepository(_dbContext);
            }
            
            return _userSettingsRepository;
        }
    }

    public ILastTimeUserReadChatRepository LastTimeUserReadChatRepository
    {
        get
        {
            if (this._lastTimeUserReadChatRepository == null)
            {
                this._lastTimeUserReadChatRepository = new LastTimeUserReadChatRepository(_dbContext);
            }
            
            return _lastTimeUserReadChatRepository;
        }
    }

    public void SaveChanges()
    {
        _dbContext.SaveChanges();
    }
}
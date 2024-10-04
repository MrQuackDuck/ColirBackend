using Colir.BLL.Interfaces;
using Colir.BLL.Misc;
using DAL.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Colir.BLL.Factories;

/// <summary>
/// Factory to get IRoomCleaner instance
/// </summary>
public class RoomCleanerFactory : IRoomCleanerFactory
{
    public IRoomCleaner GetRoomCleaner(string roomGuid, IUnitOfWork unitOfWork, IConfiguration config)
    {
        return new RoomCleaner(roomGuid, unitOfWork, config);
    }
}
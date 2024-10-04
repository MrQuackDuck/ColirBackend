using DAL.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Colir.BLL.Interfaces;

public interface IRoomCleanerFactory
{
    IRoomCleaner GetRoomCleaner(string roomGuid, IUnitOfWork unitOfWork, IConfiguration config);
}
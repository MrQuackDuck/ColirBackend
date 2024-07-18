namespace Colir.BLL.Interfaces;

public interface IRoomCleanerFactory
{
    IRoomCleaner GetRoomCleaner(string roomGuid);
}
namespace Colir.BLL.Interfaces;

public interface IClearProcessFactory
{
    IRoomCleaner GetClearProcessForRoom(string roomGuid);
}
using Colir.BLL.Interfaces;
using Colir.BLL.Misc;

namespace Colir.BLL.Factories;

/// <summary>
/// Factory to get IRoomCleaner instance
/// </summary>
public class RoomCleanerFactory : IRoomCleanerFactory
{
    public IRoomCleaner GetRoomCleaner(string directoryPath)
    {
        return new RoomCleaner(directoryPath);
    }
}
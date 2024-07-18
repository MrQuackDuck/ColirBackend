using Colir.BLL.Interfaces;
using Colir.BLL.Misc;

namespace Colir.BLL.Factories;

/// <summary>
/// Factory to get IClearProcess instance
/// </summary>
public class ClearProcessFactory : IClearProcessFactory
{
    public IRoomCleaner GetClearProcessForRoom(string directoryPath)
    {
        return new RoomCleaner(directoryPath);
    }
}
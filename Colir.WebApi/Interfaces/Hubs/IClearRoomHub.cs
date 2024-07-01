namespace Colir.Interfaces.Hubs;

public interface IClearRoomHub
{
    Task Connect(string roomGuid);

    Task Clear();
}
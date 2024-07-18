namespace Colir.BLL.Interfaces;

public interface IRoomCleaner
{
    public event Action FileDeleted;
    public int FilesToDeleteCount { get; }

    void Start();
}
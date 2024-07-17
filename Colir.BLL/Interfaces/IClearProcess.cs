namespace Colir.BLL.Interfaces;

public interface IClearProcess
{
    public event Action FileDeleted;
    public int FilesToDeleteCount { get; }

    void Start();
}
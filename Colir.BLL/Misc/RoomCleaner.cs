using Colir.BLL.Interfaces;
using Colir.Exceptions.NotFound;
using DAL.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Colir.BLL.Misc;

/// <inheritdoc cref="IRoomCleaner"/>
public class RoomCleaner : IRoomCleaner
{
    public int FilesToDeleteCount { get; }
    public event Action? FileDeleted;
    public event Action? Finished;

    private readonly List<string> _filesToDelete;
    private readonly IUnitOfWork _unitOfWork;

    public RoomCleaner(string roomGuid, IUnitOfWork unitOfWork, IConfiguration config)
    {
        var folderName = Path.Combine(config["AppSettings:RoomFilesFolderName"]!, roomGuid);
        _filesToDelete = Directory.GetFiles(folderName).ToList();
        _unitOfWork = unitOfWork;
        FilesToDeleteCount = _filesToDelete.Count;
    }

    /// <inheritdoc cref="IRoomCleaner.StartAsync"/>
    public async Task StartAsync()
    {
        foreach (var file in _filesToDelete)
        {
            try
            {
                await _unitOfWork.AttachmentRepository.DeleteAttachmentByPathAsync(file);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (AttachmentNotFoundException) { /* ignored */ }

            File.Delete(file);
            FileDeleted?.Invoke();
        }

        Finished?.Invoke();
    }
}
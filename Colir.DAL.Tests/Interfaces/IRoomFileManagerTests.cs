namespace Colir.DAL.Tests.Interfaces;

public interface IRoomFileManagerTests
{
    Task GetFileAsync_ReturnsFileAsync();

    Task GetFreeStorageSizeAsync_ReturnsFreeStorageSize();

    Task GetFilesSizeAsync_ReturnsFilesTotalSize();

    Task UploadFileAsync_UploadsFile();

    Task DeleteFileAsync_DeletesTheFileRelatedToRoom();

    Task DeleteAllFilesAsync_DeletesAllFilesRelatedToRoom();
}
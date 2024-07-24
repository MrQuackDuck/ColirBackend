namespace Colir.DAL.Tests.Interfaces;

public interface IRoomFileManagerTests
{
    Task GetFile_ReturnsFileAsync();

    Task GetFreeStorageSize_ReturnsFreeStorageSize();

    Task GetFilesSize_ReturnsFilesTotalSize();

    Task UploadFileAsync_UploadsFile();

    Task DeleteFile_DeletesTheFileRelatedToRoom();

    Task DeleteAllFiles_DeletesAllFilesRelatedToRoom();
}
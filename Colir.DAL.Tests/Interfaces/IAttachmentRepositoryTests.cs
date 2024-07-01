namespace Colir.DAL.Tests.Interfaces;

public interface IAttachmentRepositoryTests
{
    void GetAllAsync_ReturnsAllAttachments();

    void GetByIdAsync_ReturnsAttachment_WhenFound();
    void GetByIdAsync_ThrowsNotFoundException_WhenAttachmentWasNotFound();

    void AddAsync_AddsNewAttachment();
    void AddAsync_ReturnsAddedAttachment();
    void AddAsync_ThrowsNotFoundException_WhenMessageWasNotFound();

    void Delete_DeletesAttachment();
    void Delete_DeletesFile();
    void Delete_ThrowsNotFoundException_WhenAttachmentDoesNotExist();

    void DeleteByIdAsync_DeletesAttachment();
    void DeleteByIdAsync_DeletesFile();
    void DeleteByIdAsync_ThrowsNotFoundException_WhenAttachmentWasNotFoundById();

    void Update_UpdatesAttachment();
    void Update_ThrowsNotFoundException_WhenAttachmentDoesNotExist();

    void SaveChanges_SavesChanges();
}
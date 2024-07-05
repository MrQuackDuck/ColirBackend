namespace Colir.DAL.Tests.Interfaces;

public interface IAttachmentRepositoryTests
{
    Task GetAllAsync_ReturnsAllAttachments();

    Task GetByIdAsync_ReturnsAttachment_WhenFound();
    Task GetByIdAsync_ThrowsNotFoundException_WhenAttachmentWasNotFound();

    Task AddAsync_AddsNewAttachment();
    Task AddAsync_ReturnsAddedAttachment();
    Task AddAsync_ThrowsNotFoundException_WhenMessageWasNotFound();

    Task Delete_DeletesAttachment();
    Task Delete_DeletesFile();
    Task Delete_ThrowsNotFoundException_WhenAttachmentDoesNotExist();

    Task DeleteByIdAsync_DeletesAttachment();
    Task DeleteByIdAsync_DeletesFile();
    Task DeleteByIdAsync_ThrowsNotFoundException_WhenAttachmentWasNotFoundById();

    Task Update_UpdatesAttachment();
    Task Update_ThrowsNotFoundException_WhenAttachmentDoesNotExist();
}
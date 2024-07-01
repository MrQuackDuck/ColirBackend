using Colir.DAL.Tests.Interfaces;

namespace Colir.DAL.Tests.Tests;

public class AttachmentRepositoryTests : IAttachmentRepositoryTests
{
    [Test]
    public void GetAllAsync_ReturnsAllAttachments()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetByIdAsync_ReturnsAttachment_WhenFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void GetByIdAsync_ThrowsNotFoundException_WhenAttachmentWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_AddsNewAttachment()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ReturnsAddedAttachment()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void AddAsync_ThrowsNotFoundException_WhenMessageWasNotFound()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_DeletesAttachment()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_DeletesFile()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Delete_ThrowsNotFoundException_WhenAttachmentDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteByIdAsync_DeletesAttachment()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteByIdAsync_DeletesFile()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void DeleteByIdAsync_ThrowsNotFoundException_WhenAttachmentWasNotFoundById()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Update_UpdatesAttachment()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void Update_ThrowsNotFoundException_WhenAttachmentDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [Test]
    public void SaveChanges_SavesChanges()
    {
        throw new NotImplementedException();
    }
}